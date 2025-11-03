using API_healthyMind.Data;
using API_healthyMind.Repositorios.Implementacion;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine("Cadena de conexión cargada: " + connStr);

builder.Services.AddScoped<IAprendizFichaRepository, AprendizFichaRepository>();
builder.Services.AddScoped<IAprendizRepository, AprendizRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<ICategoriaPreguntasRepository, CategoriaPreguntasRepository>();
builder.Services.AddScoped<ICategoriaRespuestasRepository, CategoriaRespuestasRepository>();
builder.Services.AddScoped<ICentroRepository, CentroRepository>();
builder.Services.AddScoped<ICitasRepository, CitasRepository>();
builder.Services.AddScoped<ICiudadRepository, CiudadRepository>();
builder.Services.AddScoped<IDiarioRepository, DiarioRepository>();
builder.Services.AddScoped<IEmocionesRepository, EmocionesRepository>();
builder.Services.AddScoped<IEstadoAprendizRepository, EstadoAprendizRepository>();
builder.Services.AddScoped<IFichaRepository, FichaRepository>();
builder.Services.AddScoped<INivelFormacionRepository, NivelFormacionRepository>();
builder.Services.AddScoped<IPaginaDiarioRepository, PaginaDiarioRepository>();
builder.Services.AddScoped<IPreguntasRepository, PreguntasRepository>();
builder.Services.AddScoped<IProgramaFormacionRepository, ProgramaFormacionRepository>();
builder.Services.AddScoped<IPsicologoRepository, PsicologoRepository>();
builder.Services.AddScoped<IRegionalRepository, RegionalRepository>();
builder.Services.AddScoped<IRespuestasRepository, RespuestaRepository>();
builder.Services.AddScoped<ISeguimientoAprendizRepository, SeguimientoAprendizRepository>();
builder.Services.AddScoped<ITestGeneralRepository, TestGeneralRepository>();
builder.Services.AddScoped<ITestPreguntasRepository, TestPreguntasRepository>();


builder.Services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();


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
