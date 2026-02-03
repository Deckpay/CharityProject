using Application.Interfaces;
using Web.Components;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 1. Kiolvassuk az URL-t az appsettings.json-bõl
var apiBaseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl");

// 2. Regisztráljuk a HttpClient-et a kiolvasott URL-lel
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl ?? "https://localhost:7161/api/")
});

// 3. Regisztráljuk a Web-es AuthService-t (a postást)
// Fontos: a Web projektben lévõ AuthService osztályt add meg itt!
builder.Services.AddScoped<IAuthService, AuthApiService>();
builder.Services.AddScoped<IProductService, ProductApiService>();

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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
