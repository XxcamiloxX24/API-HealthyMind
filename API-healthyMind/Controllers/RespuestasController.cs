using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CualquierRol")]
    public class RespuestasController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public RespuestasController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }
        // GET: NivelFormacionController
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.Respuestas.ObtenerTodoConCondicion(c => c.ResEstadoRegistro == "activo",
                c => c.Include(a => a.CategoriaRespuesta));
            return Ok(datos);
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPost]
        public async Task<IActionResult> CrearRegistro([FromBody] RespuestasDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("El cuerpo no puede ser nulo");
            }

            var nuevoReg = new Respuestas
            {
                ResCategoriaFk = dto.ResCategoriaFk,
                ResDescripcion = dto.ResDescripcion
            };

            await _uow.Respuestas.Agregar(nuevoReg);
            await _uow.SaveChangesAsync();

            return Ok(nuevoReg);
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("editar/{id}")]
        public async Task<IActionResult> editarRegistro(int id, [FromBody] RespuestasDTO dto)
        {
            if (id.ToString() == "")
            {
                return BadRequest("El campo id no puede estar vacio!");
            }

            if (dto == null)
            {
                return BadRequest("El cuerpo no puede estar nulo");
            }

            var regEncontrado = (await _uow.Respuestas.ObtenerTodoConCondicion(c => c.ResCodigo == id 
            && c.ResEstadoRegistro == "activo")).FirstOrDefault();

            if (regEncontrado == null || regEncontrado.ResEstadoRegistro == "inactivo")
            {
                return NotFound("No se encontró el registro con el id solicitado");
            }

            regEncontrado.ResCategoriaFk = dto.ResCategoriaFk;
            regEncontrado.ResDescripcion = dto.ResDescripcion;


            _uow.Respuestas.Actualizar(regEncontrado);
            await _uow.SaveChangesAsync();

            return Ok(regEncontrado);

        }
        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("eliminar/{id}")]
        public async Task<IActionResult> eliminarRegistro (int id)
        {
            var regEncontrado = (await _uow.Respuestas.ObtenerTodoConCondicion(c => c.ResCodigo == id
            && c.ResEstadoRegistro == "activo")).FirstOrDefault();

            if (regEncontrado == null || regEncontrado.ResEstadoRegistro == "inactivo")
            {
                return NotFound("No se encontró el registro con el id solicitado");
            }

            regEncontrado.ResEstadoRegistro = "inactivo";


            _uow.Respuestas.Actualizar(regEncontrado);
            await _uow.SaveChangesAsync();

            return Ok("Se ha eliminado correctamente!");

        }

        
    }
}
