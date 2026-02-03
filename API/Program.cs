using Application.Interfaces;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Adatbázis elérése
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CharityDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Szervizek regisztrálása a rétegekbõl

// A UnitOfWork regisztrálása (ez kell az AuthService-nek)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// A Generic Repository regisztrálása
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// AZ IAuthService REGISZTRÁLÁSA - Ez oldja meg a hibádat!
// Figyelj rá, hogy az Infrastructure-ben lévõ AuthService-t add meg
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();

// 3. CORS beállítása (Hogy a Web projekt elérje az API-t)
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorPolicy", policy =>
        policy.WithOrigins("https://localhost:7001") // Ide a Web projekt URL-je jön
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("BlazorPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
