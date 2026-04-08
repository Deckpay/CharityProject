using Application.Interfaces;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. ADATBÁZIS ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ErtekmentoDbContext>(options =>
    options.UseSqlServer(connectionString));

//adatbázis műveletek regisztrálása
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ILookupService, LookupService>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IProductRequestService, ProductRequestService>();
builder.Services.AddScoped<ILimitService, LimitService>();


// --- DINAMIKUS CORS BEÁLLÍTÁS (Módosítva) ---
// Megpróbáljuk kiolvasni az appsettings-ből, ha nincs ott, a 7189-et használjuk alapértelmezettként
var allowedWebOrigin = builder.Configuration.GetValue<string>("AllowedWebOrigin") ?? "https://localhost:7189";

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorPolicy", policy =>
        policy.WithOrigins(allowedWebOrigin)
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var jwtKey = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication("Bearer")
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey!))
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Charity API", Version = "v1" });

    // Biztonsági definíció hozzáadása (ez hozza létre a gombot)
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization fejléc Bearer sémával. Példa: \"Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Biztonsági követelmény hozzáadása (hogy minden végpontnál ott legyen a lakat)
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Fontos: a CORS-nak az Auth előtt kell lennie!
app.UseCors("BlazorPolicy"); 
app.UseStaticFiles(); // fontos a sorrend
app.UseAuthentication(); // fontos a sorrend
app.UseAuthorization(); // fontos a sorrend

app.MapControllers();

app.Run();
