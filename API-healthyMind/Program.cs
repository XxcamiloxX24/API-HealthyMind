using API_healthyMind.Data;
using API_healthyMind.Repositorios.Implementacion;
using API_healthyMind.Repositorios.Interfaces;
using API_healthyMind.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine("Cadena de conexión cargada: " + connStr);

builder.Services.AddMyFeatureServices();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
        policy.WithOrigins("http://127.0.0.1:7197")
    .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
    connStr,
    ServerVersion.AutoDetect(connStr)
)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
