using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
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
            var datos = await _uow.ProgramaFormacion.ObtenerTodoConCondicion(e => e.ProgEstadoRegistro == "activo" && e.ProgCodigo == id,
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

            return Ok(datos);
        }

        [HttpGet("nombre/{nombre}")]
        public async Task<IActionResult> ObtenerPorNombre(string nombre)
        {
            var datos = await _uow.ProgramaFormacion.ObtenerTodoConCondicion(e => e.ProgEstadoRegistro == "activo" && e.ProgNombre.ToLower() == nombre.ToLower(),
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

            return Ok(datos);
        }

        [HttpGet("modalidad/{nombre}")]
        public async Task<IActionResult> ObtenerPorModalidad(string nombre)
        {
            var datos = await _uow.ProgramaFormacion.ObtenerTodoConCondicion(e => e.ProgEstadoRegistro == "activo" && e.ProgModalidad.ToLower() == nombre.ToLower(),
                e => e.Include(c => c.Area)
                            .ThenInclude(c => c.AreaPsicologo)
                        .Include(c => c.Centro)
                            .ThenInclude(c => c.Regional)
                        .Include(c => c.NivelFormacion)
                );

            if (datos == null || !datos.Any())
            {
                return NotFound("No se encontró ningún elemento");
            }

            return Ok(datos);
        }

        [HttpGet("formaModalidad/{nombre}")]
        public async Task<IActionResult> ObtenerPorformaModalidad(string nombre)
        {
            var datos = await _uow.ProgramaFormacion.ObtenerTodoConCondicion(e => e.ProgEstadoRegistro == "activo" && e.ProgFormaModalidad.ToLower() == nombre.ToLower(),
                e => e.Include(c => c.Area)
                            .ThenInclude(c => c.AreaPsicologo)
                        .Include(c => c.Centro)
                            .ThenInclude(c => c.Regional)
                        .Include(c => c.NivelFormacion)
                );

            if (datos == null || !datos.Any())
            {
                return NotFound("No se encontró ningún elemento");
            }

            return Ok(datos);
        }

        [HttpGet("area/{nombre}")]
        public async Task<IActionResult> ObtenerPorArea(string nombre)
        {
            var datos = await _uow.ProgramaFormacion.ObtenerTodoConCondicion(e => e.ProgEstadoRegistro == "activo" && e.Area.AreaNombre.ToLower() == nombre.ToLower(),
                e => e.Include(c => c.Area)
                            .ThenInclude(c => c.AreaPsicologo)
                        .Include(c => c.Centro)
                            .ThenInclude(c => c.Regional)
                        .Include(c => c.NivelFormacion)
                );

            if (datos == null || !datos.Any())
            {
                return NotFound("No se encontró ningún elemento");
            }

            return Ok(datos);
        }

        [HttpGet("nivelFormacion/{nombre}")]
        public async Task<IActionResult> ObtenerPornivelFormacion(string nombre)
        {
            var datos = await _uow.ProgramaFormacion.ObtenerTodoConCondicion(e => e.ProgEstadoRegistro == "activo" && e.NivelFormacion.NivForNombre.ToLower() == nombre.ToLower(),
                e => e.Include(c => c.Area)
                            .ThenInclude(c => c.AreaPsicologo)
                        .Include(c => c.Centro)
                            .ThenInclude(c => c.Regional)
                        .Include(c => c.NivelFormacion)
                );

            if (datos == null || !datos.Any())
            {
                return NotFound("No se encontró ningún elemento");
            }

            return Ok(datos);
        }

        [HttpGet("centro/{id}")]
        public async Task<IActionResult> ObtenerPorCentro(int id)
        {
            var datos = await _uow.ProgramaFormacion.ObtenerTodoConCondicion(e => e.ProgEstadoRegistro == "activo" && e.Centro.CenCodigo == id,
                e => e.Include(c => c.Area)
                            .ThenInclude(c => c.AreaPsicologo)
                        .Include(c => c.Centro)
                            .ThenInclude(c => c.Regional)
                        .Include(c => c.NivelFormacion)
                );

            if (datos == null || !datos.Any())
            {
                return NotFound("No se encontró ningún elemento");
            }

            return Ok(datos);
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
