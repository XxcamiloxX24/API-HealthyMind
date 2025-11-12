using API_healthyMind.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CiudadController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public CiudadController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }
        // GET: NivelFormacionController
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.Ciudad.ObtenerConRegional(e => e.Include(c => c.Regional));
            return Ok(datos);
        }

        [HttpGet("regional/{id}")]
        public async Task<IActionResult> ObtenerPorRegional(int id)
        {
            var datos = await _uow.Ciudad.ObtenerPorRegional(e => e.Regional.RegCodigo == id,
                e => e.Include(c => c.Regional));
            return Ok(datos);
        }

       
    }
}
