using API_healthyMind.Data;
using API_healthyMind.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static API_healthyMind.Controllers.AprendizController;

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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var reg = await _uow.CardsInfo.ObtenerPorID(id);
            if (reg == null)
                return NotFound("No se encontró esta card.");
            return Ok(reg);
        }

        /// <summary>
        /// Lista cards paginadas para el psicólogo.
        /// </summary>
        
        [HttpGet("paginado")]
        public async Task<IActionResult> ObtenerPaginado([FromQuery] PaginacionDTO p)
        {
            if (p.TamanoPagina > 100)
                p.TamanoPagina = 100;

            var query = _uow.CardsInfo.Query()
                .OrderByDescending(c => c.carCodigo);

            var totalRegistros = await query.CountAsync();

            var datos = await query
                .Skip((p.Pagina - 1) * p.TamanoPagina)
                .Take(p.TamanoPagina)
                .ToListAsync();

            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)p.TamanoPagina);

            if (p.Pagina <= 0)
                return BadRequest("La página debe ser mayor a 0.");
            if (totalPaginas > 0 && p.Pagina > totalPaginas)
                return NotFound($"La página {p.Pagina} no existe. Total: {totalPaginas}");

            return Ok(new
            {
                paginaActual = p.Pagina,
                paginaAnterior = p.Pagina > 1 ? p.Pagina - 1 : (int?)null,
                paginaSiguiente = p.Pagina < totalPaginas ? p.Pagina + 1 : (int?)null,
                tamanoPagina = p.TamanoPagina,
                totalRegistros,
                totalPaginas,
                resultados = datos
            });
        }

        /// <summary>
        /// Búsqueda dinámica por título o descripción. Mínimo 3 caracteres.
        /// </summary>
        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet("busqueda-dinamica")]
        public async Task<IActionResult> BusquedaDinamica([FromQuery] string texto)
        {
            if (string.IsNullOrWhiteSpace(texto) || texto.Length < 3)
                return BadRequest("Debe escribir al menos 3 caracteres.");

            var textoLower = texto.Trim().ToLower();

            var datos = await _uow.CardsInfo.Query()
                .Where(c =>
                    (c.carTitulo != null && c.carTitulo.ToLower().Contains(textoLower)) ||
                    (c.carDescripcion != null && c.carDescripcion.ToLower().Contains(textoLower)))
                .OrderByDescending(c => c.carCodigo)
                .ToListAsync();

            return Ok(datos);
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPost]
        public async Task<IActionResult> crearReg([FromBody] CardsInfo card)
        {
            var nuevoReg = new CardsInfo
            {
                carTitulo = card.carTitulo,
                carDescripcion = card.carDescripcion,
                carImagenUrl = card.carImagenUrl,
                carLink = card.carLink,
                carEstadoRegistro = card.carEstadoRegistro ?? "activo"
            };

            await _uow.CardsInfo.Agregar(nuevoReg);
            await _uow.SaveChangesAsync();
            return Ok(nuevoReg);
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("editar/{id}")]
        public async Task<IActionResult> editarReg(int id, [FromBody] CardsInfo card)
        {
            if (id <= 0)
            {
                return BadRequest("El id no debe ir vacío!");
            }
            var regObtenido = await _uow.CardsInfo.ObtenerPorID(id);
            if (regObtenido == null)
            {
                return NotFound("No se encontró este id");
            }

            regObtenido.carTitulo = card.carTitulo;
            regObtenido.carDescripcion = card.carDescripcion;
            regObtenido.carImagenUrl = card.carImagenUrl;
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

        /// <summary>
        /// Alterna el estado (activo/inactivo) de una card. No requiere body.
        /// </summary>
        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("estado/{id}")]
        public async Task<IActionResult> cambiarEstado(int id)
        {
            if (id <= 0)
                return BadRequest("El id no debe ir vacío.");

            var reg = await _uow.CardsInfo.ObtenerPorID(id);
            if (reg == null)
                return NotFound("No se encontró esta card.");

            var actual = (reg.carEstadoRegistro ?? "activo").Trim().ToLower();
            reg.carEstadoRegistro = actual == "activo" ? "inactivo" : "activo";
            _uow.CardsInfo.Actualizar(reg);
            await _uow.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Estado actualizado correctamente.",
                regObtenido = reg
            });
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> eliminarReg(int id)
        {
            if (id <= 0)
            {
                return BadRequest("El campo id no debe ir vacío.");
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
