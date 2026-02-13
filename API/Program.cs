using Application.Interfaces;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. ADATBÁZIS ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CharityDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- 2. SZERVIZEK REGISZTRÁLÁSA ---
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();

// --- 3. DINAMIKUS CORS BEÁLLÍTÁS (Módosítva) ---
// Megpróbáljuk kiolvasni az appsettings-bõl, ha nincs ott, a 7189-et használjuk alapértelmezettként
var allowedWebOrigin = builder.Configuration.GetValue<string>("AllowedWebOrigin") ?? "https://localhost:7189";

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorPolicy", policy =>
        policy.WithOrigins(allowedWebOrigin)
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 4. MIDDLEWARE CSATORNA ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Fontos: a CORS-nak az Auth elõtt kell lennie!
app.UseCors("BlazorPolicy");

app.UseAuthorization();
app.MapControllers();

app.Run();