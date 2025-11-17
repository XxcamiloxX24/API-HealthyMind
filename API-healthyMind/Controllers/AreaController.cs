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
    public class AreaController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public AreaController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }
        // GET: NivelFormacionController
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.Area.ObtenerTodoConPsic(e => e.AreaEstadoRegistro == "activo",
                e => e.Include(c => c.AreaPsicologo));

            if (!datos.Any())
            {
                return NotFound("No se encontró ningun area");
            }
            return Ok(datos);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var datos = await _uow.Area.ObtenerTodoConPsic(e => e.AreaCodigo == id && e.AreaEstadoRegistro == "activo",
                e => e.Include(c => c.AreaPsicologo));

            if (!datos.Any())
            {
                return NotFound("No se encontró esta area");
            }

            return Ok(datos);
        }

        [HttpGet("nombre/{nombre}")]
        public async Task<IActionResult> ObtenerPorNombre(string nombre)
        {
            var datos = await _uow.Area.ObtenerTodoConPsic(e => e.AreaNombre.ToLower() == nombre.ToLower() && e.AreaEstadoRegistro == "activo",
                e => e.Include(c => c.AreaPsicologo));

            if (!datos.Any())
            {
                return NotFound("No se encontró esta area");
            }

            return Ok(datos);
        }

        [HttpPost]
        public async Task<IActionResult> CrearArea([FromBody] AreaDTO nuevaArea)
        {

            if (nuevaArea == null)
            {
                return BadRequest("El cuerpo no puede ser nulo.");
            }

            var areaNew = new Area
            {
                AreaNombre = nuevaArea.AreaNombre,
                AreaPsicCodFk = nuevaArea.PsicologoCodigo
            };
            await _uow.Area.Agregar(areaNew);
            await _uow.SaveChangesAsync();

            

            var datos = await _uow.Area.ObtenerTodoConPsic(e => e.AreaCodigo == areaNew.AreaCodigo && e.AreaEstadoRegistro == "activo",
                e => e.Include(c => c.AreaPsicologo));

            return Ok(new
            {
                mensaje = "Área creada correctamente.",
                datos
            });
        }

        [HttpPut("editar/{id}")]
        public async Task<IActionResult> EditarArea(int id, [FromBody] AreaDTO areaRecibida)
        {
            if (id.ToString() == "")
            {
                return BadRequest("El ID no debe ser nulo");
            }
            var areaEncontrada = await _uow.Area.ObtenerPorID(id);
            
            if (areaEncontrada == null)
            {
                return NotFound("No se encontró este ID");
            }

            areaEncontrada.AreaNombre = areaRecibida.AreaNombre;
            areaEncontrada.AreaPsicCodFk = areaRecibida.PsicologoCodigo;
            

            _uow.Area.Actualizar(areaEncontrada);
            await _uow.SaveChangesAsync();

            var datos = await _uow.Area.ObtenerTodoConPsic(e => e.AreaCodigo == areaEncontrada.AreaCodigo && e.AreaEstadoRegistro == "activo",
                e => e.Include(c => c.AreaPsicologo));

            return Ok(new
            {
                mensaje = "Área editada correctamente.",
                datos
            });
        }



        [HttpPut("eliminar/{id}")]
        public async Task<IActionResult> EliminarArea(int id)
        {
            var areaEncontrada = await _uow.Area.ObtenerPorID(id);
            if (areaEncontrada.AreaEstadoRegistro == "inactivo" || areaEncontrada == null)
            {
                return NotFound("No se encontró este ID");
            }
            areaEncontrada.AreaEstadoRegistro = "inactivo";

            _uow.Area.Actualizar(areaEncontrada);
            await _uow.SaveChangesAsync();

            return Ok("Se ha eliminado correctamente!");
        }
        
    }
}
