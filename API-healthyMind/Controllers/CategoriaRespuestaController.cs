using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdministradorYPsicologo")]
    public class CategoriaRespuestaController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public CategoriaRespuestaController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }
        // GET: NivelFormacionController
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.CategoriaRespuestas.ObtenerTodos();
            return Ok(datos);
        }

        [HttpPost]
        public async Task<IActionResult> crearRegistro([FromBody] CategoriaRespuestaDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("El cuerpo no puede ser nulo");
            }

            var nuevoReg = new CategoriaRespuestas
            {
                CatResNombre = dto.CatResNombre,
                CatResDescripcion = dto.CatResDescripcion
            };

            await _uow.CategoriaRespuestas.Agregar(nuevoReg);
            await _uow.SaveChangesAsync();

            var datos = await _uow.CategoriaRespuestas.ObtenerPorID(nuevoReg.CatResCodigo);
            return Ok(datos);
        }

        [HttpPut("editar/{id}")]
        public async Task<IActionResult> editarRegistro(int id, [FromBody] CategoriaRespuestaDTO nuevo)
        {
            if (id.ToString() == "" || id.ToString() == null)
            {
                return BadRequest("El campo id no puede estar vacio!");
            }

            if (nuevo == null)
            {
                return BadRequest("El cuerpo no puede ser nulo");
            }

            var regEncontrado = await _uow.CategoriaRespuestas.ObtenerPorID(id);

            if (regEncontrado == null)
            {
                return NotFound("No se encontró este id");
            }

            regEncontrado.CatResNombre = nuevo.CatResNombre;
            regEncontrado.CatResDescripcion = nuevo.CatResDescripcion;

            _uow.CategoriaRespuestas.Actualizar(regEncontrado);
            await _uow.SaveChangesAsync();

            return Ok(regEncontrado);
        }


        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> eliminarRegistro(int id)
        {
            var regEncontrado = await _uow.CategoriaRespuestas.ObtenerPorID(id);

            if (regEncontrado == null)
            {
                return NotFound("No se encontró este id");
            }

            _uow.CategoriaRespuestas.Eliminar(regEncontrado);
            await _uow.SaveChangesAsync();

            return Ok("Se ha eliminado correctamente!");
        }

    }
}
