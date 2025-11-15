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
    public class FichaController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public FichaController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }
        // GET: NivelFormacionController
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.Ficha.ObtenerTodoConCondicion(e => e.FicEstadoRegistro == "activo",
                e => e.Include(c => c.programaFormacion)
                        .ThenInclude(c => c.Area)
                            .ThenInclude(c => c.AreaPsicologo)
                        .Include(c => c.programaFormacion.Centro)
                            .ThenInclude(c => c.Regional)
                        .Include(c => c.programaFormacion.NivelFormacion));

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            return Ok(datos);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var datos = await _uow.Ficha.ObtenerTodoConCondicion(e => e.FicEstadoRegistro == "activo" && e.FicCodigo == id,
                e => e.Include(c => c.programaFormacion)
                        .ThenInclude(c => c.Area)
                            .ThenInclude(c => c.AreaPsicologo)
                        .Include(c => c.programaFormacion.Centro)
                            .ThenInclude(c => c.Regional)
                        .Include(c => c.programaFormacion.NivelFormacion));

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró el registro!");
            }

            return Ok(datos);
        }

        [HttpGet("jornada/{jornada}")]
        public async Task<IActionResult> ObtenerPorJornada(string jornada)
        {
            var datos = await _uow.Ficha.ObtenerTodoConCondicion(e => e.FicEstadoRegistro == "activo" && e.FicJornada.ToLower() == jornada.ToLower(),
                                                                    e => e.Include(c => c.programaFormacion)
                                                                            .ThenInclude(c => c.Area)
                                                                                .ThenInclude(c => c.AreaPsicologo)
                                                                            .Include(c => c.programaFormacion.Centro)
                                                                                .ThenInclude(c => c.Regional)
                                                                            .Include(c => c.programaFormacion.NivelFormacion));

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            return Ok(datos);
        }

        [HttpGet("estadoFormacion/{estadoFormacion}")]
        public async Task<IActionResult> ObtenerPorEstadoFormacion(string programaFormacion)
        {
            var datos = await _uow.Ficha.ObtenerTodoConCondicion(e => e.FicEstadoRegistro == "activo" && e.FicEstadoFormacion.ToLower() == programaFormacion.ToLower(),
                                                                    e => e.Include(c => c.programaFormacion)
                                                                            .ThenInclude(c => c.Area)
                                                                                .ThenInclude(c => c.AreaPsicologo)
                                                                            .Include(c => c.programaFormacion.Centro)
                                                                                .ThenInclude(c => c.Regional)
                                                                            .Include(c => c.programaFormacion.NivelFormacion));

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            return Ok(datos);
        }

        [HttpGet("programaFormacion/{programaFormacion}")]
        public async Task<IActionResult> ObtenerPorProgramaFormacion(int area)
        {
            var datos = await _uow.Ficha.ObtenerTodoConCondicion(e => e.FicEstadoRegistro == "activo" && e.programaFormacion.ProgCodigo == area,
                                                                    e => e.Include(c => c.programaFormacion)
                                                                            .ThenInclude(c => c.Area)
                                                                                .ThenInclude(c => c.AreaPsicologo)
                                                                            .Include(c => c.programaFormacion.Centro)
                                                                                .ThenInclude(c => c.Regional)
                                                                            .Include(c => c.programaFormacion.NivelFormacion));

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            return Ok(datos);
        }

        [HttpGet("area/{area}")]
        public async Task<IActionResult> ObtenerPorArea(int area)
        {
            var datos = await _uow.Ficha.ObtenerTodoConCondicion(e => e.FicEstadoRegistro == "activo" && e.programaFormacion.Area.AreaCodigo == area,
                                                                    e => e.Include(c => c.programaFormacion)
                                                                            .ThenInclude(c => c.Area)
                                                                                .ThenInclude(c => c.AreaPsicologo)
                                                                            .Include(c => c.programaFormacion.Centro)
                                                                                .ThenInclude(c => c.Regional)
                                                                            .Include(c => c.programaFormacion.NivelFormacion));

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            return Ok(datos);
        }

        [HttpGet("psicologo/{id}")]
        public async Task<IActionResult> ObtenerPorPsicologo(int id)
        {
            var datos = await _uow.Ficha.ObtenerTodoConCondicion(e => e.FicEstadoRegistro == "activo" && e.programaFormacion.Area.AreaPsicologo.PsiDocumento == id,
                                                                    e => e.Include(c => c.programaFormacion)
                                                                            .ThenInclude(c => c.Area)
                                                                                .ThenInclude(c => c.AreaPsicologo)
                                                                            .Include(c => c.programaFormacion.Centro)
                                                                                .ThenInclude(c => c.Regional)
                                                                            .Include(c => c.programaFormacion.NivelFormacion));

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            return Ok(datos);
        }

        [HttpGet("centro/{id}")]
        public async Task<IActionResult> ObtenerPorCentro(int id)
        {
            var datos = await _uow.Ficha.ObtenerTodoConCondicion(e => e.FicEstadoRegistro == "activo" && e.programaFormacion.Centro.CenCodigo == id,
                                                                    e => e.Include(c => c.programaFormacion)
                                                                            .ThenInclude(c => c.Area)
                                                                                .ThenInclude(c => c.AreaPsicologo)
                                                                            .Include(c => c.programaFormacion.Centro)
                                                                                .ThenInclude(c => c.Regional)
                                                                            .Include(c => c.programaFormacion.NivelFormacion));

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            return Ok(datos);
        }

        [HttpPost]
        public async Task<IActionResult> CrearArea([FromBody] FichaDTO nuevaFicha)
        {

            if (nuevaFicha == null)
            {
                return BadRequest("El cuerpo no puede ser nulo.");
            }

            var fichaNew = new Ficha
            {
                FicCodigo = nuevaFicha.FicCodigo,
                FicJornada = nuevaFicha.FicJornada,
                FicFechaInicio = nuevaFicha.FicFechaInicio,
                FicFechaFin = nuevaFicha.FicFechaFin,
                FicEstadoFormacion = nuevaFicha.FicEstadoFormacion,
                FicProgramaFk = nuevaFicha.FicProgramaFk
            };
            await _uow.Ficha.Agregar(fichaNew);
            await _uow.SaveChangesAsync();



            var datos = await _uow.Ficha.ObtenerTodoConCondicion(e => e.FicEstadoRegistro == "activo" && e.FicCodigo == fichaNew.FicCodigo,
                                                                    e => e.Include(c => c.programaFormacion)
                                                                            .ThenInclude(c => c.Area)
                                                                                .ThenInclude(c => c.AreaPsicologo)
                                                                            .Include(c => c.programaFormacion.Centro)
                                                                                .ThenInclude(c => c.Regional)
                                                                            .Include(c => c.programaFormacion.NivelFormacion));

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró el registro!");
            }

            return Ok(new
            {
                mensaje = "Ficha creada correctamente.",
                datos
            });
        }

        [HttpPut("editar/{id}")]
        public async Task<IActionResult> EditarFicha(int id, [FromBody] FichaDTO fichaRecibida)
        {
            if (id.ToString() == "")
            {
                return BadRequest("El ID no debe ser nulo");
            }
            var fichaEncontrada = await _uow.Ficha.ObtenerPorID(id);
            
            if (fichaEncontrada == null)
            {
                return NotFound("No se encontró este ID");
            }

            fichaEncontrada.FicCodigo = fichaRecibida.FicCodigo;
            fichaEncontrada.FicJornada = fichaRecibida.FicJornada;
            fichaEncontrada.FicFechaInicio = fichaRecibida.FicFechaInicio;
            fichaEncontrada.FicFechaFin = fichaRecibida.FicFechaFin;
            fichaEncontrada.FicEstadoFormacion = fichaRecibida.FicEstadoFormacion;
            fichaEncontrada.FicProgramaFk = fichaRecibida.FicProgramaFk;



            _uow.Ficha.Actualizar(fichaEncontrada);
            await _uow.SaveChangesAsync();

            var datos = await _uow.Ficha.ObtenerTodoConCondicion(e => e.FicEstadoRegistro == "activo" && e.FicCodigo == fichaEncontrada.FicCodigo,
                                                                    e => e.Include(c => c.programaFormacion)
                                                                            .ThenInclude(c => c.Area)
                                                                                .ThenInclude(c => c.AreaPsicologo)
                                                                            .Include(c => c.programaFormacion.Centro)
                                                                                .ThenInclude(c => c.Regional)
                                                                            .Include(c => c.programaFormacion.NivelFormacion));

            return Ok(new
            {
                mensaje = "Ficha editada correctamente.",
                datos
            });
        }



        [HttpPut("eliminar/{id}")]
        public async Task<IActionResult> EliminarFicha(int id)
        {
            var fichaEncontrada = await _uow.Ficha.ObtenerPorID(id);
            if (fichaEncontrada.FicEstadoRegistro == "inactivo" || fichaEncontrada == null)
            {
                return NotFound("No se encontró este ID");
            }
            fichaEncontrada.FicEstadoRegistro = "inactivo";

            _uow.Ficha.Actualizar(fichaEncontrada);
            await _uow.SaveChangesAsync();

            return Ok("Se ha eliminado correctamente!");
        }
        
    }
}
