using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmocionesController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public EmocionesController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }
        // GET: EmocionesController
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.Emociones.ObtenerTodos();
            return Ok(datos);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorid(int id)
        {
            var datos = await _uow.Emociones.ObtenerPorID(id);
            return Ok(datos);
        }

        [HttpPost]
        public async Task<IActionResult> CrearEmocion([FromBody] EmocionesDTO emocion)
        {
            if (emocion == null)
            {
                return BadRequest("El cuerpo no puede ser nulo");
            }

            var nuevaEmocion = new Emociones
            {
                EmoNombre = emocion.EmoNombre,
                EmoDescripcion = emocion.EmoDescripcion,
                EmoImage = emocion.EmoImage
            };

            await _uow.Emociones.Agregar(nuevaEmocion);
            await _uow.SaveChangesAsync();

            var datos = await _uow.Emociones.ObtenerPorID(nuevaEmocion.EmoCodigo);
            return Ok(new
            {
                mensaje = "Se ha creado correctamente!",
                datos
            });
        }

        [HttpPut("editar/{id}")]
        public async Task<IActionResult> EditarEmocion(int id,[FromBody] EmocionesDTO emoNueva)
        {
            if (id.ToString() == "")
            {
                return BadRequest("El campo id no debe estar vacio");
            }
            if (emoNueva == null)
            {
                return BadRequest("El cuerpo no debe estar nulo");
            }

            var emoEncontrada = await _uow.Emociones.ObtenerPorID(id);

            if (emoEncontrada == null)
            {
                return NotFound("No se encontró este id");
            }
            emoEncontrada.EmoNombre = emoNueva.EmoNombre;
            emoEncontrada.EmoDescripcion = emoNueva.EmoDescripcion;
            emoEncontrada.EmoImage = emoNueva.EmoImage;

            _uow.Emociones.Actualizar(emoEncontrada);
            await _uow.SaveChangesAsync(); 
            return Ok(emoEncontrada);
        }

        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> eliminarEmo(int id)
        {
            var emoEncontrada = await _uow.Emociones.ObtenerPorID(id);
            if (emoEncontrada == null)
            {
                return NotFound("No se encontró este id!");
            }

            _uow.Emociones.Eliminar(emoEncontrada);
            await _uow.SaveChangesAsync();

            return Ok("Se ha eliminado correctamente!");
        }
    }
}
