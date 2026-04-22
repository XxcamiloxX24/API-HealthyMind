using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CualquierRol")]
    public class NivelFormacionController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public NivelFormacionController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.NivelFormacion.ObtenerTodos();
            return Ok(datos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var item = await _uow.NivelFormacion.ObtenerPrimero(e => e.NivForCodigo == id);
            if (item == null)
                return NotFound("No se encontró el nivel de formación.");
            return Ok(item);
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] NivelFormacionDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.NivForNombre))
                return BadRequest("El nombre es obligatorio.");

            var entity = new NivelFormacion
            {
                NivForNombre = dto.NivForNombre.Trim(),
                NivForDescripcion = string.IsNullOrWhiteSpace(dto.NivForDescripcion)
                    ? null
                    : dto.NivForDescripcion.Trim()
            };
            await _uow.NivelFormacion.Agregar(entity);
            await _uow.SaveChangesAsync();
            return Ok(new { mensaje = "Nivel creado correctamente.", datos = entity });
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("editar/{id:int}")]
        public async Task<IActionResult> Editar(int id, [FromBody] NivelFormacionDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.NivForNombre))
                return BadRequest("El nombre es obligatorio.");

            var entity = await _uow.NivelFormacion.ObtenerPrimero(e => e.NivForCodigo == id);
            if (entity == null)
                return NotFound("No se encontró el nivel de formación.");

            entity.NivForNombre = dto.NivForNombre.Trim();
            entity.NivForDescripcion = string.IsNullOrWhiteSpace(dto.NivForDescripcion)
                ? null
                : dto.NivForDescripcion.Trim();

            _uow.NivelFormacion.Actualizar(entity);
            await _uow.SaveChangesAsync();
            return Ok(new { mensaje = "Nivel actualizado correctamente.", datos = entity });
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var entity = await _uow.NivelFormacion.ObtenerPrimero(e => e.NivForCodigo == id);
            if (entity == null)
                return NotFound("No se encontró el nivel de formación.");

            var enUso = await _uow.ProgramaFormacion.Query().AnyAsync(p =>
                p.ProgNivFormFk == id && p.ProgEstadoRegistro == "activo");
            if (enUso)
                return Conflict("No se puede eliminar: hay programas de formación activos que usan este nivel.");

            _uow.NivelFormacion.Eliminar(entity);
            await _uow.SaveChangesAsync();
            return Ok("Nivel eliminado correctamente.");
        }
    }
}
