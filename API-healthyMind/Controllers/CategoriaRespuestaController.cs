using API_healthyMind.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

       
    }
}
