using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreguntasController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public PreguntasController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }
        // GET: NivelFormacionController
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.Preguntas.ObtenerTodoConCondicion(c => c.PregEstadoRegistro == "activo",
                c => c.Include(a => a.CategoriaPregunta));
            return Ok(datos);
        }

        [HttpPost]
        public async Task<IActionResult> CrearRegistro([FromBody] PreguntasDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("El cuerpo no puede ser nulo");
            }

            var nuevoReg = new Preguntas
            {
                PregCategoriaFk = dto.PregCategoriaFk,
                PregDescripcion = dto.PregDescripcion
            };

            await _uow.Preguntas.Agregar(nuevoReg);
            await _uow.SaveChangesAsync();

            return Ok(nuevoReg);
        }

        [HttpPut("editar/{id}")]
        public async Task<IActionResult> editarRegistro(int id, [FromBody] PreguntasDTO dto)
        {
            if (id.ToString() == "")
            {
                return BadRequest("El campo id no puede estar vacio!");
            }

            if (dto == null)
            {
                return BadRequest("El cuerpo no puede estar nulo");
            }

            var regEncontrado = (await _uow.Preguntas.ObtenerTodoConCondicion(c => c.PregCodigo == id
            && c.PregEstadoRegistro == "activo")).FirstOrDefault();

            if (regEncontrado == null || regEncontrado.PregEstadoRegistro == "inactivo")
            {
                return NotFound("No se encontró el registro con el id solicitado");
            }

            regEncontrado.PregCategoriaFk = dto.PregCategoriaFk;
            regEncontrado.PregDescripcion = dto.PregDescripcion;


            _uow.Preguntas.Actualizar(regEncontrado);
            await _uow.SaveChangesAsync();

            return Ok(regEncontrado);

        }
        [HttpPut("eliminar/{id}")]
        public async Task<IActionResult> eliminarRegistro(int id)
        {
            var regEncontrado = (await _uow.Preguntas.ObtenerTodoConCondicion(c => c.PregCodigo == id
            && c.PregEstadoRegistro == "activo")).FirstOrDefault();

            if (regEncontrado == null || regEncontrado.PregEstadoRegistro == "inactivo")
            {
                return NotFound("No se encontró el registro con el id solicitado");
            }

            regEncontrado.PregEstadoRegistro = "inactivo";


            _uow.Preguntas.Actualizar(regEncontrado);
            await _uow.SaveChangesAsync();

            return Ok("Se ha eliminado correctamente!");

        }
    }
}
