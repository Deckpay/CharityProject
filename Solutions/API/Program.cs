using Application.Interfaces;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Hubs;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- ADATBÁZIS ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ErtekmentoDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- DEPENDENCY INJECTION (Services + Repositories) ---
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ILookupService, LookupService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IProductRequestService, ProductRequestService>();
builder.Services.AddScoped<ILimitService, LimitService>();

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();

// --- CORS BEÁLLÍTÁS ---
// Frontend elérésének engedélyezése
var allowedWebOrigin = builder.Configuration.GetValue<string>("AllowedWebOrigin") ?? "https://localhost:7189";

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorPolicy", policy =>
        policy.WithOrigins(allowedWebOrigin)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// --- AUTHENTICATION (JWT) ---
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication("Bearer")
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey!))
    };
});

// --- CONTROLLERS + SWAGGER ---

builder.Services.AddControllers();
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

// --- MIDDLEWARE PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Fontos a sorrend!
app.UseCors("BlazorPolicy"); // 1.
app.UseStaticFiles(); // 2.
app.UseAuthentication(); // 3.
app.UseAuthorization(); // 4.

app.MapHub<ChatHub>("/chathub");

app.MapControllers();

app.Run();
