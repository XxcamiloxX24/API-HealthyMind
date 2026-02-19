using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Models.DTO.Filtros;
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
    [Authorize(Policy = "CualquierRol")]
    public class ProgramaFormacionController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public ProgramaFormacionController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }

        private static object MapearPrograma(Programaformacion d)
        {
            return new
                {
                    d.ProgCodigo,
                    d.ProgNombre,
                    d.ProgModalidad,
                    d.ProgFormaModalidad,
                    d.NivelFormacion,
                    Area = d.Area == null ? null : new
                    {
                        d.Area.AreaCodigo,
                        d.Area.AreaNombre,
                        Psicologo = d.Area.AreaPsicologo,
                        
                    },
                    Centro = d.Centro== null ? null : new
                    {
                        d.Centro.CenCodigo,
                        d.Centro.CenNombre,
                        d.Centro.CenDireccion,
                        Regional = new
                        {
                            d.Centro.Regional.RegCodigo,
                            d.Centro.Regional.RegNombre
                        },
                        d.Centro.CenCodFk
                    }
                };
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.ProgramaFormacion.ObtenerTodoConCondicion(e => e.ProgEstadoRegistro == "activo",
                e => e.Include(c => c.Area)
                            .ThenInclude(c => c.AreaPsicologo)
                        .Include(c => c.Centro)
                            .ThenInclude(c => c.Regional)
                        .Include(c => c.NivelFormacion)
                );
            var resultado = datos.Select(MapearPrograma);

            return Ok(resultado);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var datos = await _uow.ProgramaFormacion.ObtenerTodoConCondicion(e => e.ProgEstadoRegistro == "activo" && e.ProgCodigo == id,
                e => e.Include(c => c.Area)
                            .ThenInclude(c => c.AreaPsicologo)
                        .Include(c => c.Centro)
                            .ThenInclude(c => c.Regional)
                        .Include(c => c.NivelFormacion)
                );

            if (datos == null || !datos.Any())
            {
                return NotFound("No se encontró este elemento");
            }
            var resultado = datos.Select(MapearPrograma);
            return Ok(resultado);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] FiltrosProgramasDTO f)
        {
            IQueryable<Programaformacion> q = _uow.ProgramaFormacion.Query()
                .Where(x => x.ProgEstadoRegistro.ToLower() == "activo")
                .Include(x => x.Area)
                    .ThenInclude(a => a.AreaPsicologo)
                .Include(x => x.Centro)
                    .ThenInclude(c => c.Regional)
                .Include(x => x.NivelFormacion);

            // ===========================
            //        F I L T R O S
            // ===========================

            if (!string.IsNullOrEmpty(f.ProgramaNombre))
                q = q.Where(x => x.ProgNombre.ToLower().Contains(f.ProgramaNombre.ToLower()));

            if (!string.IsNullOrEmpty(f.AreaNombre))
                q = q.Where(x => x.Area.AreaNombre.ToLower().Contains(f.AreaNombre.ToLower()));

            if (!string.IsNullOrEmpty(f.CentroNombre))
                q = q.Where(x => x.Centro.CenNombre.ToLower().Contains(f.CentroNombre.ToLower()));

            if (!string.IsNullOrEmpty(f.NivelFormacion))
                q = q.Where(x => x.NivelFormacion.NivForNombre.ToLower().Contains(f.NivelFormacion.ToLower()));

            // 🔥 FILTRO QUE PEDISTE: DOCUMENTO DEL PSICÓLOGO
            if (!string.IsNullOrEmpty(f.PsicologoDocumento))
                q = q.Where(x => x.Area.AreaPsicologo.PsiDocumento.ToLower() == f.PsicologoDocumento.ToLower());

            if (!string.IsNullOrEmpty(f.Modalidad))
                q = q.Where(x => x.ProgModalidad.ToLower().Contains(f.Modalidad.ToLower()));

            if (!string.IsNullOrEmpty(f.FormaModalidad))
                q = q.Where(x => x.ProgFormaModalidad.ToLower().Contains(f.FormaModalidad.ToLower()));

            // ===========================
            //       EJECUCIÓN
            // ===========================

            var datos = await q.ToListAsync();

            if (!datos.Any())
                return NotFound("No se encontraron programas con estos filtros.");

            var resultado = datos.Select(MapearPrograma);

            return Ok(resultado);
        }




        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPost]
        public async Task<IActionResult> CrearPrograma([FromBody] ProgramaFormacionDTO nuevoPrograma)
        {

            if (nuevoPrograma == null)
            {
                return BadRequest("El cuerpo no puede ser nulo.");
            }

            var programNew = new Programaformacion
            {
                ProgNombre = nuevoPrograma.ProgNombre,
                ProgModalidad = nuevoPrograma.ProgModalidad,
                ProgFormaModalidad = nuevoPrograma.ProgFormaModalidad,
                ProgNivFormFk = nuevoPrograma.ProgNivFormFk,
                ProgAreaFk = nuevoPrograma.ProgAreaFk,
                ProgCentroFk = nuevoPrograma.ProgCentroFk
            };
            await _uow.ProgramaFormacion.Agregar(programNew);
            await _uow.SaveChangesAsync();



            var datos = await _uow.ProgramaFormacion.ObtenerTodoConCondicion(e => e.ProgEstadoRegistro == "activo" && e.ProgCodigo == programNew.ProgCodigo,
                e => e.Include(c => c.Area)
                            .ThenInclude(c => c.AreaPsicologo)
                        .Include(c => c.Centro)
                            .ThenInclude(c => c.Regional)
                        .Include(c => c.NivelFormacion)
                );

            if (datos == null || !datos.Any())
            {
                return NotFound("No se encontró este elemento");
            }
            var resultado = datos.Select(MapearPrograma);
            return Ok(new
            {
                mensaje = "Programa creado correctamente!",
                resultado
            });
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("editar/{id}")]
        public async Task<IActionResult> EditarPrograma(int id, [FromBody] ProgramaFormacionDTO programaRecibido)
        {
            if (id.ToString() == "")
            {
                return BadRequest("El ID no debe ser nulo");
            }
            var programaEncontrado = await _uow.ProgramaFormacion.ObtenerPorID(id);
            
            if (programaEncontrado == null || programaEncontrado.ProgEstadoRegistro == "inactivo")
            {
                return NotFound("No se encontró este ID");
            }

            programaEncontrado.ProgNombre = programaRecibido.ProgNombre;
            programaEncontrado.ProgModalidad = programaRecibido.ProgModalidad;
            programaEncontrado.ProgFormaModalidad = programaRecibido.ProgFormaModalidad;
            programaEncontrado.ProgNivFormFk = programaRecibido.ProgNivFormFk;
            programaEncontrado.ProgAreaFk = programaRecibido.ProgAreaFk;
            programaEncontrado.ProgCentroFk = programaRecibido.ProgCentroFk;

            _uow.ProgramaFormacion.Actualizar(programaEncontrado);
            await _uow.SaveChangesAsync();

            var datos = await _uow.ProgramaFormacion.ObtenerTodoConCondicion(e => e.ProgEstadoRegistro == "activo" && e.ProgCodigo == programaEncontrado.ProgCodigo,
                e => e.Include(c => c.Area)
                            .ThenInclude(c => c.AreaPsicologo)
                        .Include(c => c.Centro)
                            .ThenInclude(c => c.Regional)
                        .Include(c => c.NivelFormacion)
                );

            if (datos == null || !datos.Any())
            {
                return NotFound("No se encontró este elemento");
            }
            var resultado = datos.Select(MapearPrograma);
            return Ok(new
            {
                mensaje = "Programa editado correctamente!",
                resultado
            });
        }



        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("eliminar/{id}")]
        public async Task<IActionResult> EliminarPrograma(int id)
        {
            var programaEncontrado = await _uow.ProgramaFormacion.ObtenerPorID(id);
            if (programaEncontrado.ProgEstadoRegistro == "inactivo" || programaEncontrado == null)
            {
                return NotFound("No se encontró este ID");
            }
            programaEncontrado.ProgEstadoRegistro = "inactivo";

            _uow.ProgramaFormacion.Actualizar(programaEncontrado);
            await _uow.SaveChangesAsync();

            return Ok("Se ha eliminado correctamente!");
        }
        
    }
}
