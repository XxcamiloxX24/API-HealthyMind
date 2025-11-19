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
    public class TestPreguntasController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public TestPreguntasController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }
        
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.ProgramaFormacion.ObtenerTodoConCondicion(e => e.ProgEstadoRegistro == "activo",
                e => e.Include(c => c.Area)
                            .ThenInclude(c => c.AreaPsicologo)
                        .Include(c => c.Centro)
                            .ThenInclude(c => c.Regional)
                        .Include(c => c.NivelFormacion)
                );

            
            return Ok(datos);
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
