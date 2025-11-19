using API_healthyMind.Data;
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

       
    }
}
