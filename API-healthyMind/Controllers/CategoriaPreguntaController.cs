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
    public class CategoriaPreguntaController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public CategoriaPreguntaController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }
        // GET: NivelFormacionController
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.CategoriaPreguntas.ObtenerTodos();
            return Ok(datos);
        }

        [HttpPost]
        public async Task<IActionResult> crearRegistro([FromBody] CategoriaPreguntasDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("El cuerpo no puede ser nulo");
            }

            var nuevoReg = new CategoriaPreguntas
            {
                CatPregNombre = dto.CatPregNombre,
                CatPregDescripcion = dto.CatPregDescripcion
            };

            await _uow.CategoriaPreguntas.Agregar(nuevoReg);
            await _uow.SaveChangesAsync();

            var datos = await _uow.CategoriaPreguntas.ObtenerPorID(nuevoReg.CatPregCodigo);
            return Ok(datos);
        }

        [HttpPut("editar/{id}")]
        public async Task<IActionResult> editarRegistro(int id, [FromBody]CategoriaPreguntasDTO nuevo)
        {
            if (id.ToString() == "" || id.ToString() == null)
            {
                return BadRequest("El campo id no puede estar vacio!");
            }

            if (nuevo == null)
            {
                return BadRequest("El cuerpo no puede ser nulo");
            }

            var regEncontrado = await _uow.CategoriaPreguntas.ObtenerPorID(id);

            if (regEncontrado == null)
            {
                return NotFound("No se encontró este id");
            }

            regEncontrado.CatPregNombre = nuevo.CatPregNombre;
            regEncontrado.CatPregDescripcion= nuevo.CatPregDescripcion;

            _uow.CategoriaPreguntas.Actualizar(regEncontrado);
            await _uow.SaveChangesAsync();

            return Ok(regEncontrado);
        }


        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> eliminarRegistro(int id)
        {
            var regEncontrado = await _uow.CategoriaPreguntas.ObtenerPorID(id);

            if (regEncontrado == null)
            {
                return NotFound("No se encontró este id");
            }

            _uow.CategoriaPreguntas.Eliminar(regEncontrado);
            await _uow.SaveChangesAsync();

            return Ok("Se ha eliminado correctamente!");
        }

    }
}
