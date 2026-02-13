using Application.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Web.Components;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. BLAZOR ALAPOK ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// --- 2. HTTPCLIENT ÉS API URL (Módosítva) ---
// Kiolvassuk az appsettings.json-bõl az API címét
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7161/api/";

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

// --- 3. SAJÁT SZERVIZEK ---
// A Web-es implementációk (amik az API-t hívják)
builder.Services.AddScoped<IAuthService, AuthApiService>();
builder.Services.AddScoped<IProductService, ProductApiService>();

// Auth és LocalStorage kezelés
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();

var app = builder.Build();

// --- 4. MIDDLEWARE CSATORNA ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
