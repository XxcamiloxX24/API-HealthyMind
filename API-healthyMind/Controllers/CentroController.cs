using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdministradorYPsicologo")]
    public class CentroController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public CentroController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }
        // GET: NivelFormacionController
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.Centro.ObtenerPorRegional(e => e.CenEstadoRegistro == "activo",e => e.Include(c => c.Regional));

            var resultado = datos.Select(c => new
            {
                c.CenCodigo,
                c.CenNombre,
                c.CenDireccion,
                Regional = new
                {
                    c.Regional.RegCodigo,
                    c.Regional.RegNombre
                },
                c.CenCodFk
            });
            return Ok(resultado);
        }

        [HttpGet("regional/{id}")]
        public async Task<IActionResult> ObtenerPorRegional(int id)
        {
            var datos = await _uow.Centro.ObtenerPorRegional(e => e.Regional.RegCodigo == id,
                e => e.Include(c => c.Regional));
            var resultado = datos.Select(c => new
            {
                c.CenCodigo,
                c.CenNombre,
                c.CenDireccion,
                Regional = new
                {
                    c.Regional.RegCodigo,
                    c.Regional.RegNombre
                },
                c.CenCodFk
            });
            return Ok(resultado);
        }

        [HttpGet("nodos")]
        public async Task<IActionResult> obtenerPorNodos(int id)
        {
            if (id.ToString() == "")
            {
                return BadRequest("El campo id no debe estar vacio!");
            }
            var regObtenidos = await _uow.Centro.ObtenerTodoConCondicion(c => c.CenCodFk == id, 
                c=> c.Include(x => x.InverseCenCodFkNavigation),
                c=> c.Include(x => x.Regional)
                
                );

            if (!regObtenidos.Any())
            {
                return NotFound("No se encontró ningun nodo que pertenezca a este centro!");
            }


            var resultado = regObtenidos.Select(c => new
            {
                c.CenCodigo,
                c.CenNombre,
                c.CenDireccion,
                Regional = new
                {
                    c.Regional.RegCodigo,
                    c.Regional.RegNombre
                },
                c.CenCodFk
            });
            return Ok(resultado);

        }   


        [HttpPost]
        public async Task<IActionResult> CrearCentro([FromBody] CentroDTO nuevoCentro)
        {

            if (nuevoCentro == null)
            {
                return BadRequest("El cuerpo no puede ser nulo.");
            }

            var centroNew = new Centro
            {
                CenNombre = nuevoCentro.CenNombre,
                CenDireccion = nuevoCentro.CenDireccion,
                CenRegCodFk = nuevoCentro.CenRegCodFk,
                CenCodFk = null
            };
            await _uow.Centro.Agregar(centroNew);
            await _uow.SaveChangesAsync();

            if (nuevoCentro.CenCodFk == 0)
            {
                centroNew.CenCodFk = centroNew.CenCodigo;
                _uow.Centro.Actualizar(centroNew);
                await _uow.SaveChangesAsync();
            }

            var centroCompleto = (await _uow.Centro.ObtenerConRegional(q =>
                    q.Include(c => c.Regional).Where(c => c.CenCodigo == centroNew.CenCodigo)))
                    .FirstOrDefault();
            var resultado = new  
            {
                CenCodigo = centroNew.CenCodigo,
                CenNombre = centroNew.CenNombre,
                CenDireccion = centroNew.CenDireccion,
                Regional = new
                {
                    centroCompleto?.Regional?.RegCodigo,
                    centroCompleto?.Regional?.RegNombre
                },
                CenCodFk = centroNew?.CenCodFk
            };
            return Ok(resultado);
        }

        [HttpPut("editar/{id}")]
        public async Task<IActionResult> EditarCentro(int id, [FromBody] CentroDTO centroRecibido)
        {
            if (id == null)
            {
                return BadRequest("El ID no debe ser nulo");
            }
            var centros = await _uow.Centro.ObtenerPorIdWithCondition(e => e.CenCodigo == id && e.CenEstadoRegistro == "activo",
                                                                e => e.Include(c => c.Regional));
            var centroEncontrado = centros.FirstOrDefault();
            if (centroEncontrado == null)
            {
                return NotFound("No se encontró este ID");
            }

            centroEncontrado.CenNombre = centroRecibido.CenNombre;
            centroEncontrado.CenDireccion = centroRecibido.CenDireccion;
            centroEncontrado.CenRegCodFk = centroRecibido.CenRegCodFk;
            centroEncontrado.CenCodFk = centroRecibido.CenCodFk;

            _uow.Centro.Actualizar(centroEncontrado);
            await _uow.SaveChangesAsync();

            var centroActualizado = (await _uow.Centro.ObtenerConRegional(q =>
        q.Include(c => c.Regional)
         .Where(c => c.CenCodigo == centroEncontrado.CenCodigo)))
        .FirstOrDefault();

            var resultado = new
            {
                centroActualizado.CenCodigo,
                centroActualizado.CenNombre,
                centroActualizado.CenDireccion,
                Regional = new
                {
                    centroActualizado?.Regional?.RegCodigo,
                    centroActualizado?.Regional?.RegNombre
                },
                centroActualizado?.CenCodFk
            };

            return Ok(resultado);
        }



        [HttpPut("eliminar/{id}")]
        public async Task<IActionResult> EliminarCentro(int id)
        {
            var centroEncontrado = await _uow.Centro.ObtenerPorID(id);
            if (centroEncontrado.CenEstadoRegistro == "inactivo" || centroEncontrado == null)
            {
                return NotFound("No se encontró este ID");
            }
            centroEncontrado.CenEstadoRegistro = "inactivo";

            _uow.Centro.Actualizar(centroEncontrado);
            await _uow.SaveChangesAsync();

            return Ok("Se ha eliminado correctamente!");
        }
    }
}
