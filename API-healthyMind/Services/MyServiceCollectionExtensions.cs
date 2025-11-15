using API_healthyMind.Data;
using API_healthyMind.Repositorios.Implementacion;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Services
{
    public static class MyServiceCollectionExtensions
    {
        public static IServiceCollection AddMyFeatureServices(this IServiceCollection services)
        {
            services.AddScoped<IAprendizFichaRepository, AprendizFichaRepository>();
            services.AddScoped<IAprendizRepository, AprendizRepository>();
            services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
            services.AddScoped<IAreaRepository, AreaRepository>();
            services.AddScoped<ICategoriaPreguntasRepository, CategoriaPreguntasRepository>();
            services.AddScoped<ICategoriaRespuestasRepository, CategoriaRespuestasRepository>();
            services.AddScoped<ICentroRepository, CentroRepository>();
            services.AddScoped<ICitasRepository, CitasRepository>();
            services.AddScoped<ICiudadRepository, CiudadRepository>();
            services.AddScoped<IDiarioRepository, DiarioRepository>();
            services.AddScoped<IEmocionesRepository, EmocionesRepository>();
            services.AddScoped<IEstadoAprendizRepository, EstadoAprendizRepository>();
            services.AddScoped<IFichaRepository, FichaRepository>();
            services.AddScoped<INivelFormacionRepository, NivelFormacionRepository>();
            services.AddScoped<IPaginaDiarioRepository, PaginaDiarioRepository>();
            services.AddScoped<IPreguntasRepository, PreguntasRepository>();
            services.AddScoped<IProgramaFormacionRepository, ProgramaFormacionRepository>();
            services.AddScoped<IPsicologoRepository, PsicologoRepository>();
            services.AddScoped<IRegionalRepository, RegionalRepository>();
            services.AddScoped<IRespuestasRepository, RespuestaRepository>();
            services.AddScoped<ISeguimientoAprendizRepository, SeguimientoAprendizRepository>();
            services.AddScoped<ITestGeneralRepository, TestGeneralRepository>();
            services.AddScoped<ITestPreguntasRepository, TestPreguntasRepository>();

            services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();

            services.AddScoped<IEmailService, EmailService>();

            services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                        options.JsonSerializerOptions.WriteIndented = true;
                    });
            return services;
        }
    }
}
