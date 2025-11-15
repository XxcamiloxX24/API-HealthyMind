using API_healthyMind.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_healthyMind.Data
{
    public class UnidadDeTrabajo : IUnidadDeTrabajo, IDisposable
    {
        private readonly AppDbContext _appDbContext;

        public IAprendizFichaRepository AprendizFicha {  get; }
        public IVerificationCodeRepository VerificationCode { get; }
        public IAprendizRepository Aprendiz {  get; }
        public IAreaRepository Area {  get; }
        public ICategoriaPreguntasRepository CategoriaPreguntas { get; }
        public ICategoriaRespuestasRepository CategoriaRespuestas { get; }
        public ICentroRepository Centro { get; }
        public ICitasRepository Citas { get; }
        public ICiudadRepository Ciudad { get; }
        public IDiarioRepository Diario { get; }
        public IEmocionesRepository Emociones { get; }
        public IEstadoAprendizRepository EstadoAprendiz { get; }
        public IFichaRepository Ficha { get; }
        public INivelFormacionRepository NivelFormacion { get; }
        public IPaginaDiarioRepository PaginaDiario { get; }
        public IPreguntasRepository Preguntas { get; }
        public IProgramaFormacionRepository ProgramaFormacion { get; }
        public IPsicologoRepository Psicologo { get; }
        public IRegionalRepository Regional { get; }
        public IRespuestasRepository Respuestas { get; }
        public ISeguimientoAprendizRepository SeguimientoAprendiz { get; }
        public ITestGeneralRepository TestGeneral { get; }
        public ITestPreguntasRepository TestPreguntas { get; }

        public UnidadDeTrabajo(AppDbContext appDbContext, IAprendizFichaRepository aprendizFicha,
            IAprendizRepository aprendiz,
            IVerificationCodeRepository verificationCode,
            IAreaRepository area,
            ICategoriaPreguntasRepository categoriaPreguntas,
            ICategoriaRespuestasRepository categoriaRespuestas, 
            ICentroRepository centro, 
            ICitasRepository citas, 
            ICiudadRepository ciudad, 
            IDiarioRepository diario, 
            IEmocionesRepository emociones, 
            IEstadoAprendizRepository estadoAprendiz, 
            IFichaRepository ficha, 
            INivelFormacionRepository nivelFormacion, 
            IPaginaDiarioRepository paginaDiario, 
            IPreguntasRepository preguntas, 
            IProgramaFormacionRepository programaFormacion, 
            IPsicologoRepository psicologo, 
            IRegionalRepository regional, 
            IRespuestasRepository respuestas, 
            ISeguimientoAprendizRepository seguimientoAprendiz, 
            ITestGeneralRepository testGeneral, 
            ITestPreguntasRepository testPreguntas)
        {
            _appDbContext = appDbContext;
            AprendizFicha = aprendizFicha;
            Aprendiz = aprendiz;
            VerificationCode = verificationCode;
            Area = area;
            CategoriaPreguntas = categoriaPreguntas;
            CategoriaRespuestas = categoriaRespuestas;
            Centro = centro;
            Citas = citas;
            Ciudad = ciudad;
            Diario = diario;
            Emociones = emociones;
            EstadoAprendiz = estadoAprendiz;
            Ficha = ficha;
            NivelFormacion = nivelFormacion;
            PaginaDiario = paginaDiario;
            Preguntas = preguntas;
            ProgramaFormacion = programaFormacion;
            Psicologo = psicologo;
            Regional = regional;
            Respuestas = respuestas;
            SeguimientoAprendiz = seguimientoAprendiz;
            TestGeneral = testGeneral;
            TestPreguntas = testPreguntas;
        }

        public void Dispose() => _appDbContext.Dispose();
        public AppDbContext ObtenerContexto() => _appDbContext;

        public async Task<int> SaveChangesAsync() => await _appDbContext.SaveChangesAsync();
    }
}
