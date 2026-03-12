using Application.Interfaces;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using Web.Services;
using WEB.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Kiolvassuk az appsettings.json-ből az API címét
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
if (string.IsNullOrEmpty(apiBaseUrl))
{
    throw new Exception("ApiSettings:BaseUrl nincs beállítva az appsettings.json-ben");
}

// TokenStore és TokenHandler regisztrálása a kliensek előtt
builder.Services.AddScoped<TokenStore>();
builder.Services.AddScoped<TokenHandler>();

builder.Services.AddHttpClient<AuthApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<TokenHandler>();

// Product API Kliens felkészítése a Token küldésre
builder.Services.AddHttpClient<ProductApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<TokenHandler>();

builder.Services.AddScoped<CustomAuthStateProvider>(sp =>
    (CustomAuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

// WEB Program.cs
builder.Services.AddSingleton<TokenStore>(); // Scoped helyett Singleton

// A Web-es implementációk (amik az API-t hívják)
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
