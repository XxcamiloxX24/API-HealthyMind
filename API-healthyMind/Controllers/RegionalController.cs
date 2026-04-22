using API_healthyMind.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdministradorYPsicologo")]
    public class RegionalController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public RegionalController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.Regional.ObtenerTodos();
            return Ok(datos);
        }
    }
}
