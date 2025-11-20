using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Models.DTO.Filtros;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestGeneralController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public TestGeneralController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }

        private static object MapearAprendiz(Aprendiz d)
        {
            return new
            {
                Codigo = d.AprCodigo,
                FechaCreacion = d.AprFechaCreacion,
                TipoDocumento = d.AprTipoDocumento,
                NroDocumento = d.AprNroDocumento,
                FechaNacimiento = d.AprFechaNac,
                Nombres = new
                {
                    PrimerNombre = d.AprNombre,
                    SegundoNombre = d.AprSegundoNombre
                },
                Apellidos = new
                {
                    PrimerApellido = d.AprApellido,
                    SegundoApellido = d.AprSegundoApellido
                },
                Ubicacion = new
                {
                    DepartamentoID = d.Municipio.Regional.RegCodigo,
                    Departamento = d.Municipio.Regional.RegNombre,
                    MunicipioID = d.Municipio.CiuCodigo,
                    Municipio = d.Municipio.CiuNombre,
                    Direccion = d.AprDireccion
                },
                Contacto = new
                {
                    Telefono = d.AprTelefono,
                    CorreoInstitucional = d.AprCorreoInstitucional,
                    CorreoPersonal = d.AprCorreoPersonal,
                    Acudiente = new
                    {
                        AcudienteNombre = d.AprAcudNombre,
                        AcudienteApellido = d.AprAcudApellido,
                        AcudienteTelefono = d.AprTelefonoAcudiente
                    }
                },
                d.EstadoAprendiz,
                Eps = d.AprEps,
                Patologia = d.AprPatologia,
                TipoPoblacion = d.AprTipoPoblacion
            };
        }

        private static object MapearAprendizFicha(AprendizFicha d)
        {
            return new
            {
                d.AprFicCodigo,
                Aprendiz = MapearAprendiz(d.Aprendiz),
                Ficha = new
                {
                    d.Ficha.FicCodigo,
                    d.Ficha.FicJornada,
                    d.Ficha.FicFechaInicio,
                    d.Ficha.FicFechaFin,
                    d.Ficha.FicEstadoFormacion,
                    ProgramaFormacion = new
                    {
                        d.Ficha.programaFormacion.ProgCodigo,
                        d.Ficha.programaFormacion.ProgNombre,
                        d.Ficha.programaFormacion.ProgModalidad,
                        d.Ficha.programaFormacion.ProgFormaModalidad,
                        d.Ficha.programaFormacion.NivelFormacion,
                        Area = new
                        {
                            d.Ficha.programaFormacion.Area.AreaCodigo,
                            d.Ficha.programaFormacion.Area.AreaNombre,
                        },
                        d.Ficha.programaFormacion.Centro
                    }
                }

            };
        }

        private static object MapearTestGeneral(TestGeneral d)
        {
            return new
            {
                d.TestGenCodigo,
                AprendizTest = MapearAprendizFicha(d.TestGenApreFkNavigation),
                Psicologo = d.TestGenPsicoFkNavigation,
                FechaRealizado = d.TestGenFechaRealiz,
                Resultados = d.TestGenResultados,
                Recomendaciones = d.TestGenRecomendacion
            };
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.TestGeneral.ObtenerTodoConCondicion(e => e.TestGenEstado == "activo",
                e => e.Include(c => c.TestGenApreFkNavigation)
                    .ThenInclude(c => c.Aprendiz)
                        .ThenInclude(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                      .Include(c => c.TestGenApreFkNavigation.Aprendiz.EstadoAprendiz)
                      .Include(c => c.TestGenApreFkNavigation.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.TestGenPsicoFkNavigation)
                        );


            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            var resultados = datos.Select(c => new
            {
                c.TestGenCodigo,
                aprendiz = MapearAprendizFicha(c.TestGenApreFkNavigation),
                psicologo = c.TestGenPsicoFkNavigation,
                FechaRealizacion = c.TestGenFechaRealiz,
                Resultados = c.TestGenResultados,
                Recomendaciones = c.TestGenRecomendacion
            });

            return Ok(resultados);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var datos = await _uow.TestGeneral.ObtenerTodoConCondicion(e => e.TestGenEstado == "activo" && e.TestGenCodigo == id,
                e => e.Include(c => c.TestGenApreFkNavigation)
                    .ThenInclude(c => c.Aprendiz)
                        .ThenInclude(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                      .Include(c => c.TestGenApreFkNavigation.Aprendiz.EstadoAprendiz)
                      .Include(c => c.TestGenApreFkNavigation.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.TestGenPsicoFkNavigation)
                        );


            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            var resultados = datos.Select(c => new
            {
                c.TestGenCodigo,
                aprendiz = MapearAprendizFicha(c.TestGenApreFkNavigation),
                psicologo = c.TestGenPsicoFkNavigation,
                FechaRealizacion = c.TestGenFechaRealiz,
                Resultados = c.TestGenResultados,
                Recomendaciones = c.TestGenRecomendacion
            });

            return Ok(resultados);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] FiltroTestGeneralDTO f)
        {
            IQueryable<TestGeneral> q = _uow.TestGeneral.Query()
                .Include(c => c.TestGenApreFkNavigation)
                    .ThenInclude(c => c.Aprendiz)
                        .ThenInclude(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                      .Include(c => c.TestGenApreFkNavigation.Aprendiz.EstadoAprendiz)
                      .Include(c => c.TestGenApreFkNavigation.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.TestGenPsicoFkNavigation);

            if (f.Codigo.HasValue)
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.AprCodigo == f.Codigo.Value);

            if (!string.IsNullOrEmpty(f.TipoDocumento))
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.AprTipoDocumento == f.TipoDocumento);

            if (f.NroDocumento.HasValue)
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.AprNroDocumento == f.NroDocumento.Value);

            if (!string.IsNullOrEmpty(f.PrimerNombre))
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.AprNombre.ToLower().Contains(f.PrimerNombre.ToLower()));

            if (!string.IsNullOrEmpty(f.PrimerApellido))
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.AprApellido.ToLower().Contains(f.PrimerApellido.ToLower()));

            if (!string.IsNullOrEmpty(f.MunicipioNombre))
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.Municipio.CiuNombre.ToLower() == f.MunicipioNombre.ToLower());

            if (!string.IsNullOrEmpty(f.DepartamentoNombre))
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.Municipio.Regional.RegNombre.ToLower() == f.DepartamentoNombre.ToLower());

            if (f.EstadoAprendizID.HasValue)
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.EstadoAprendiz.EstAprCodigo == f.EstadoAprendizID.Value);

            if (!string.IsNullOrEmpty(f.Eps))
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.AprEps.ToLower() == f.Eps.ToLower());

            if (!string.IsNullOrEmpty(f.TipoPoblacion))
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.AprTipoPoblacion.ToLower() == f.TipoPoblacion.ToLower());

            if (f.PsicologoDocumento.HasValue)
                q = q.Where(x => x.TestGenPsicoFkNavigation.PsiDocumento == f.PsicologoDocumento.Value);

            if (f.FechaRealizacionDesde.HasValue)
            {
                var desde = f.FechaRealizacionDesde.Value.Date;
                q = q.Where(x => x.TestGenFechaRealiz.HasValue && x.TestGenFechaRealiz.Value.Date >= desde);
            }

            if (f.FechaRealizacionHasta.HasValue)
            {
                var hasta = f.FechaRealizacionHasta.Value.Date;
                q = q.Where(x => x.TestGenFechaRealiz.HasValue && x.TestGenFechaRealiz.Value.Date <= hasta);
            }




            var datos = await q.ToListAsync();

            if (!datos.Any())
                return NotFound("No se encontraron resultados con esos filtros.");

            var resultados = datos.Select(MapearTestGeneral);

            return Ok(resultados);
        }



        [HttpPost]
        public async Task<IActionResult> CrearPrograma([FromBody] ProgramaFormacionDTO nuevoPrograma)
        {

            if (nuevoPrograma == null)
            {
                return BadRequest("El cuerpo no puede ser nulo.");
            }

            var programNew = new Programaformacion
            {
                ProgNombre = nuevoPrograma.ProgNombre,
                ProgModalidad = nuevoPrograma.ProgModalidad,
                ProgFormaModalidad = nuevoPrograma.ProgFormaModalidad,
                ProgNivFormFk = nuevoPrograma.ProgNivFormFk,
                ProgAreaFk = nuevoPrograma.ProgAreaFk,
                ProgCentroFk = nuevoPrograma.ProgCentroFk
            };
            await _uow.ProgramaFormacion.Agregar(programNew);
            await _uow.SaveChangesAsync();



            var datos = await _uow.ProgramaFormacion.ObtenerTodoConCondicion(e => e.ProgEstadoRegistro == "activo" && e.ProgCodigo == programNew.ProgCodigo,
                e => e.Include(c => c.Area)
                            .ThenInclude(c => c.AreaPsicologo)
                        .Include(c => c.Centro)
                            .ThenInclude(c => c.Regional)
                        .Include(c => c.NivelFormacion)
                );

            if (datos == null || !datos.Any())
            {
                return NotFound("No se encontró este elemento");
            }

            return Ok(new
            {
                mensaje = "Programa creado correctamente!",
                datos
            });
        }

        [HttpPut("editar/{id}")]
        public async Task<IActionResult> EditarPrograma(int id, [FromBody] ProgramaFormacionDTO programaRecibido)
        {
            if (id.ToString() == "")
            {
                return BadRequest("El ID no debe ser nulo");
            }
            var programaEncontrado = await _uow.ProgramaFormacion.ObtenerPorID(id);
            
            if (programaEncontrado == null || programaEncontrado.ProgEstadoRegistro == "inactivo")
            {
                return NotFound("No se encontró este ID");
            }

            programaEncontrado.ProgNombre = programaRecibido.ProgNombre;
            programaEncontrado.ProgModalidad = programaRecibido.ProgModalidad;
            programaEncontrado.ProgFormaModalidad = programaRecibido.ProgFormaModalidad;
            programaEncontrado.ProgNivFormFk = programaRecibido.ProgNivFormFk;
            programaEncontrado.ProgAreaFk = programaRecibido.ProgAreaFk;
            programaEncontrado.ProgCentroFk = programaRecibido.ProgCentroFk;

            _uow.ProgramaFormacion.Actualizar(programaEncontrado);
            await _uow.SaveChangesAsync();

            var datos = await _uow.ProgramaFormacion.ObtenerTodoConCondicion(e => e.ProgEstadoRegistro == "activo" && e.ProgCodigo == programaEncontrado.ProgCodigo,
                e => e.Include(c => c.Area)
                            .ThenInclude(c => c.AreaPsicologo)
                        .Include(c => c.Centro)
                            .ThenInclude(c => c.Regional)
                        .Include(c => c.NivelFormacion)
                );

            if (datos == null || !datos.Any())
            {
                return NotFound("No se encontró este elemento");
            }

            return Ok(new
            {
                mensaje = "Programa editado correctamente!",
                datos
            });
        }



        [HttpPut("eliminar/{id}")]
        public async Task<IActionResult> EliminarPrograma(int id)
        {
            var programaEncontrado = await _uow.ProgramaFormacion.ObtenerPorID(id);
            if (programaEncontrado.ProgEstadoRegistro == "inactivo" || programaEncontrado == null)
            {
                return NotFound("No se encontró este ID");
            }
            programaEncontrado.ProgEstadoRegistro = "inactivo";

            _uow.ProgramaFormacion.Actualizar(programaEncontrado);
            await _uow.SaveChangesAsync();

            return Ok("Se ha eliminado correctamente!");
        }
        
    }
}
