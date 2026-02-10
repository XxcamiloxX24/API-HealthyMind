using API_healthyMind.Data;
using API_healthyMind.Repositorios.Implementacion;
using API_healthyMind.Repositorios.Interfaces;
using API_healthyMind.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Pkix;
using API_healthyMind.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine("Cadena de conexión cargada: " + connStr);

builder.Services.AddMyFeatureServices();

builder.Configuration.AddJsonFile("appsettings.json");
var secretkey = builder.Configuration["settings:secretkey"];
var keyBytes = Encoding.UTF8.GetBytes(secretkey);

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
    };
});

builder.Services.AddAuthorization(options =>
{
    // Solo Administrador
    options.AddPolicy("SoloAdministrador", policy =>
        policy.RequireRole(Roles.Administrador));

    // Administrador o Psicólogo
    options.AddPolicy("AdministradorYPsicologo", policy =>
        policy.RequireRole(Roles.Administrador, Roles.Psicologo));

    // Administrador o Aprendiz
    options.AddPolicy("AdministradorYAprendiz", policy =>
        policy.RequireRole(Roles.Administrador, Roles.Aprendiz));

    // Psicólogo o Aprendiz
    options.AddPolicy("PsicologoYAprendiz", policy =>
        policy.RequireRole(Roles.Psicologo, Roles.Aprendiz));

    // Cualquier usuario autenticado (los 3 roles)
    options.AddPolicy("CualquierRol", policy =>
        policy.RequireRole(
            Roles.Administrador,
            Roles.Psicologo,
            Roles.Aprendiz
        ));
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization. Ejemplo: \"Bearer {tu_token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            );
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
    connStr,
    ServerVersion.AutoDetect(connStr)
)
);

var app = builder.Build();

app.UseCors("CorsPolicy");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
