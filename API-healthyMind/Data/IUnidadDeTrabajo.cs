using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Data
{
    public interface IUnidadDeTrabajo : IDisposable
    {
        IAprendizFichaRepository AprendizFicha { get; }
        IVerificationCodeRepository VerificationCode { get; }
        IAprendizRepository Aprendiz { get; }
        IAreaRepository Area { get; }
        ICategoriaPreguntasRepository CategoriaPreguntas { get; }
        ICategoriaRespuestasRepository CategoriaRespuestas { get; }
        ICentroRepository Centro { get; }
        ICitasRepository Citas { get; }
        ICiudadRepository Ciudad { get; }
        IDiarioRepository Diario { get; }
        IEmocionesRepository Emociones { get; }
        IEstadoAprendizRepository EstadoAprendiz { get; }
        IFichaRepository Ficha {  get; }
        INivelFormacionRepository NivelFormacion { get; }
        IPaginaDiarioRepository PaginaDiario { get; }
        IPreguntasRepository Preguntas { get; }
        IProgramaFormacionRepository ProgramaFormacion { get; }
        IPsicologoRepository Psicologo {  get; }
        IRegionalRepository Regional { get; }
        IRespuestasRepository Respuestas { get; }
        ISeguimientoAprendizRepository SeguimientoAprendiz { get; }
        ITestGeneralRepository TestGeneral {  get; }
        ITestPreguntasRepository TestPreguntas { get; }
        AppDbContext ObtenerContexto();
        Task<int> SaveChangesAsync();

    }
}
