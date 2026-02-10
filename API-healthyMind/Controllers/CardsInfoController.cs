using API_healthyMind.Data;
using API_healthyMind.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CualquierRol")]
    public class CardsInfoController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public CardsInfoController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.CardsInfo.ObtenerTodos();
            if (!datos.Any())
            {
                return NotFound("No se encontraron registros!");
            }
            return Ok(datos);
        }


        [HttpGet("activos")]
        public async Task<IActionResult> ObtenerTodosActivos()
        {
            var datos = await _uow.CardsInfo.ObtenerTodoConCondicion(e => e.carEstadoRegistro == "activo");
            if (!datos.Any())
            {
                return NotFound("No se encontraron registros activos!");
            }
            return Ok(datos);
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPost]
        public async Task<IActionResult> crearReg([FromBody] CardsInfo card)
        {
            var nuevoreg = new CardsInfo
            {
                carTitulo = card.carTitulo,
                carDescripcion = card.carDescripcion,
                carLink = card.carLink,
                carEstadoRegistro = card.carEstadoRegistro
            };

            await _uow.CardsInfo.Agregar(card);
            await _uow.SaveChangesAsync();
            return Ok(card);
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("editar/{id}")]
        public async Task<IActionResult> editarReg(int id, [FromBody] CardsInfo card)
        {
            if (id.ToString() == "")
            {
                return BadRequest("El id no debe ir vacio!");
            }
            var regObtenido = await _uow.CardsInfo.ObtenerPorID(id);
            if (regObtenido == null)
            {
                return NotFound("No se encontró este id");
            }

            regObtenido.carTitulo = card.carTitulo;
            regObtenido.carDescripcion = card.carDescripcion;
            regObtenido.carLink = card.carLink;
            regObtenido.carEstadoRegistro = card.carEstadoRegistro;

            _uow.CardsInfo.Actualizar(regObtenido);
            await _uow.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Se ha editado correctamente!",
                regObtenido
            });
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> eliminarReg(int id)
        {
            if (id.ToString() == "")
            {
                return BadRequest("El campo id no debe ir vacio");
            }
            var regObtenido = await _uow.CardsInfo.ObtenerPorID(id);
            if (regObtenido == null)
            {
                return NotFound("No se encontró este id");
            }
            _uow.CardsInfo.Eliminar(regObtenido);
            await _uow.SaveChangesAsync();
            return Ok("Se ha elimado correctamente!");
        }
    }
}
