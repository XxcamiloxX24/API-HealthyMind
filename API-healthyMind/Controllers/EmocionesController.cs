using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CualquierRol")]
    public class EmocionesController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public EmocionesController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }

        private static object MapearEmocion(Emociones e) => new
        {
            e.EmoCodigo,
            e.EmoNombre,
            e.EmoEmoji,
            e.EmoEscala,
            e.EmoColorFondo,
            e.EmoDescripcion,
            e.EmoImage,
            e.EmoEstadoRegistro,
            Categoria = e.Categoria
        };

        private static string ObtenerCategoria(int escala) => escala switch
        {
            <= 2 => "Critica",
            <= 4 => "Negativa",
            <= 6 => "Neutral",
            _    => "Positiva"
        };

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.Emociones.ObtenerTodoConCondicion(
                e => e.EmoEstadoRegistro == "activo");

            return Ok(datos.Select(MapearEmocion));
        }

        [HttpGet("todos-incluido-inactivos")]
        [Authorize(Policy = "AdministradorYPsicologo")]
        public async Task<IActionResult> ObtenerTodosIncluidoInactivos()
        {
            var datos = await _uow.Emociones.ObtenerTodos();
            return Ok(datos.Select(MapearEmocion));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var emo = await _uow.Emociones.ObtenerPorID(id);
            if (emo == null)
                return NotFound("No se encontró la emoción.");

            return Ok(MapearEmocion(emo));
        }

        [HttpGet("categorias")]
        public IActionResult ObtenerCategorias()
        {
            var categorias = new[]
            {
                new { nombre = "Critica",  escalaMin = 1,  escalaMax = 2,  color = "#ef4444" },
                new { nombre = "Negativa", escalaMin = 3,  escalaMax = 4,  color = "#f97316" },
                new { nombre = "Neutral",  escalaMin = 5,  escalaMax = 6,  color = "#eab308" },
                new { nombre = "Positiva", escalaMin = 7,  escalaMax = 10, color = "#22c55e" },
            };
            return Ok(categorias);
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPost]
        public async Task<IActionResult> CrearEmocion([FromBody] EmocionesDTO dto)
        {
            if (dto == null)
                return BadRequest("El cuerpo no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(dto.EmoNombre))
                return BadRequest("El nombre de la emoción es obligatorio.");

            var escala = dto.EmoEscala ?? 5;
            if (escala < 1 || escala > 10)
                return BadRequest("La escala debe estar entre 1 y 10.");

            var nueva = new Emociones
            {
                EmoNombre = dto.EmoNombre.Trim(),
                EmoEmoji = dto.EmoEmoji?.Trim(),
                EmoEscala = escala,
                EmoColorFondo = dto.EmoColorFondo?.Trim(),
                EmoDescripcion = dto.EmoDescripcion?.Trim(),
                EmoImage = dto.EmoImage?.Trim(),
                EmoEstadoRegistro = "activo"
            };

            await _uow.Emociones.Agregar(nueva);
            await _uow.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Se ha creado correctamente!",
                datos = MapearEmocion(nueva)
            });
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("editar/{id}")]
        public async Task<IActionResult> EditarEmocion(int id, [FromBody] EmocionesDTO dto)
        {
            if (dto == null)
                return BadRequest("El cuerpo no debe estar nulo.");

            var emo = await _uow.Emociones.ObtenerPorID(id);
            if (emo == null)
                return NotFound("No se encontró este id.");

            if (dto.EmoEscala.HasValue && (dto.EmoEscala < 1 || dto.EmoEscala > 10))
                return BadRequest("La escala debe estar entre 1 y 10.");

            emo.EmoNombre = dto.EmoNombre ?? emo.EmoNombre;
            emo.EmoEmoji = dto.EmoEmoji ?? emo.EmoEmoji;
            if (dto.EmoEscala.HasValue) emo.EmoEscala = dto.EmoEscala.Value;
            emo.EmoColorFondo = dto.EmoColorFondo ?? emo.EmoColorFondo;
            emo.EmoDescripcion = dto.EmoDescripcion ?? emo.EmoDescripcion;
            emo.EmoImage = dto.EmoImage ?? emo.EmoImage;

            _uow.Emociones.Actualizar(emo);
            await _uow.SaveChangesAsync();

            return Ok(MapearEmocion(emo));
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("eliminar/{id}")]
        public async Task<IActionResult> EliminarEmocion(int id)
        {
            var emo = await _uow.Emociones.ObtenerPorID(id);
            if (emo == null)
                return NotFound("No se encontró este id!");

            emo.EmoEstadoRegistro = "inactivo";
            _uow.Emociones.Actualizar(emo);
            await _uow.SaveChangesAsync();

            return Ok("Se ha eliminado correctamente!");
        }
    }
}
