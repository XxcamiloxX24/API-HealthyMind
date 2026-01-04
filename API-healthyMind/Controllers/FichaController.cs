using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Models.DTO.Filtros;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static API_healthyMind.Controllers.AprendizController;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FichaController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public FichaController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }

        private static object MapearFicha(Ficha d)
        {
            return new
                {
                    d.FicCodigo,
                    d.FicJornada,
                    d.FicFechaInicio,
                    d.FicFechaFin,
                    d.FicEstadoFormacion,
                    ProgramaFormacion = d.programaFormacion == null ? null : new
                    {
                        d.programaFormacion.ProgCodigo,
                        d.programaFormacion.ProgNombre,
                        d.programaFormacion.ProgModalidad,
                        d.programaFormacion.ProgFormaModalidad,
                        d.programaFormacion.NivelFormacion,
                        d.programaFormacion.Area,
                        d.programaFormacion.Centro
                    }

            };
        }

        // GET: NivelFormacionController
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.Ficha.ObtenerTodoConCondicion(e => e.FicEstadoRegistro == "activo",
                e => e.Include(c => c.programaFormacion)
                        .ThenInclude(c => c.Area)
                            .ThenInclude(c => c.AreaPsicologo)
                        .Include(c => c.programaFormacion.Centro)
                            .ThenInclude(c => c.Regional)
                        .Include(c => c.programaFormacion.NivelFormacion));

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            return Ok(datos);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var datos = await _uow.Ficha.ObtenerTodoConCondicion(e => e.FicEstadoRegistro == "activo" && e.FicCodigo == id,
                e => e.Include(c => c.programaFormacion)
                        .ThenInclude(c => c.Area)
                            .ThenInclude(c => c.AreaPsicologo)
                        .Include(c => c.programaFormacion.Centro)
                            .ThenInclude(c => c.Regional)
                        .Include(c => c.programaFormacion.NivelFormacion));

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró el registro!");
            }

            return Ok(datos);
        }

        [HttpGet("listar")]
        public async Task<IActionResult> ListarFichas([FromQuery] PaginacionDTO p)
        {
            if (p.TamanoPagina > 100)
                p.TamanoPagina = 100;

            var query = _uow.Ficha.Query()
                        .Include(c => c.programaFormacion)
                            .ThenInclude(c => c.Area)
                                .ThenInclude(c => c.AreaPsicologo)
                        .Include(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                                .ThenInclude(c => c.Regional)
                        .Include(c => c.programaFormacion)
                            .ThenInclude(c => c.NivelFormacion)
                        .OrderByDescending(a => a.FicCodigo);

            var totalRegistros = await query.CountAsync();

            if (totalRegistros == 0)
                return NotFound("No existen registros.");

            int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)p.TamanoPagina);

            if (p.Pagina <= 0)
                return BadRequest("La página debe ser mayor a 0.");

            if (p.Pagina > totalPaginas)
                return NotFound($"La página {p.Pagina} no existe. Total: {totalPaginas}");

            var datos = await query
                .Skip((p.Pagina - 1) * p.TamanoPagina)
                .Take(p.TamanoPagina)
                .ToListAsync();

            

            return Ok(new
            {
                paginaActual = p.Pagina,
                paginaAnterior = p.Pagina > 1 ? p.Pagina - 1 : (int?)null,
                paginaSiguiente = p.Pagina < totalPaginas ? p.Pagina + 1 : (int?)null,
                tamanoPagina = p.TamanoPagina,
                totalRegistros,
                totalPaginas,
                datos
            });
        }

        [HttpGet("busqueda-dinamica")]
        public async Task<IActionResult> BuscarDinamico([FromQuery] string texto)
        {
            if (string.IsNullOrWhiteSpace(texto) || texto.Length < 3)
                return BadRequest("Debe escribir al menos 3 caracteres.");

            texto = texto.ToLower();

            var query = _uow.Ficha.Query()
                .Include(c => c.programaFormacion)
                    .ThenInclude(c => c.Area)
                        .ThenInclude(c => c.AreaPsicologo)
                .Include(c => c.programaFormacion)
                    .ThenInclude(c => c.Centro)
                        .ThenInclude(c => c.Regional)
                .Include(c => c.programaFormacion)
                    .ThenInclude(c => c.NivelFormacion)
                .Where(a =>
                    a.FicCodigo.ToString().ToLower().Contains(texto) ||
                    a.FicJornada.ToLower().Contains(texto) ||
                    (a.programaFormacion != null &&
                       (a.programaFormacion.ProgNombre.ToLower().Contains(texto)))
                );

            
            var datos = await query.ToListAsync();

            if (!datos.Any())
                return NotFound("No se encontraron resultados.");

            return Ok(datos);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] FiltroFichaDTO f)
        {
            IQueryable<Ficha> q = _uow.Ficha.Query()
                .Include(fic => fic.programaFormacion)
                        .ThenInclude(p => p.Area)
                            .ThenInclude(a => a.AreaPsicologo)
                .Include(fic => fic.programaFormacion)
                        .ThenInclude(p => p.NivelFormacion)
                .Include(fic => fic.programaFormacion)
                        .ThenInclude(p => p.Centro)
                            .ThenInclude(c => c.Regional);

            // ======================
            //    FILTROS
            // ======================

            if (f.FichaCodigo.HasValue)
                q = q.Where(x => x.FicCodigo == f.FichaCodigo.Value);

            if (!string.IsNullOrEmpty(f.AreaNombre))
                q = q.Where(x => x.programaFormacion.Area.AreaNombre.ToLower()
                    .Contains(f.AreaNombre.ToLower()));

            if (!string.IsNullOrEmpty(f.ProgramaNombre))
                q = q.Where(x => x.programaFormacion.ProgNombre.ToLower()
                    .Contains(f.ProgramaNombre.ToLower()));

            if (!string.IsNullOrEmpty(f.PsicologoID))
                q = q.Where(x => x.programaFormacion.Area.AreaPsicologo.PsiDocumento.ToLower() == f.PsicologoID.ToLower());

            if (!string.IsNullOrEmpty(f.CentroNombre))
                q = q.Where(x => x.programaFormacion.Centro.CenNombre.ToLower()
                    .Contains(f.CentroNombre.ToLower()));

            if (!string.IsNullOrEmpty(f.Jornada))
                q = q.Where(x => x.FicJornada.ToLower() == f.Jornada.ToLower());

            if (f.FechaInicio.HasValue)
                q = q.Where(x => x.FicFechaInicio >= f.FechaInicio.Value);

            if (f.FechaFin.HasValue)
                q = q.Where(x => x.FicFechaFin <= f.FechaFin.Value);

            q = q.Where(x => x.FicEstadoRegistro.ToLower() == "activo");



            // ======================
            //  EJECUCIÓN
            // ======================

            var datos = await q.ToListAsync();

            if (!datos.Any())
                return NotFound("No se encontraron resultados con esos filtros.");

            var resultado = datos.Select(MapearFicha);

            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> CrearArea([FromBody] FichaDTO nuevaFicha)
        {

            if (nuevaFicha == null)
            {
                return BadRequest("El cuerpo no puede ser nulo.");
            }

            var fichaNew = new Ficha
            {
                FicCodigo = nuevaFicha.FicCodigo,
                FicJornada = nuevaFicha.FicJornada,
                FicFechaInicio = nuevaFicha.FicFechaInicio,
                FicFechaFin = nuevaFicha.FicFechaFin,
                FicEstadoFormacion = nuevaFicha.FicEstadoFormacion,
                FicProgramaFk = nuevaFicha.FicProgramaFk
            };
            await _uow.Ficha.Agregar(fichaNew);
            await _uow.SaveChangesAsync();



            var datos = await _uow.Ficha.ObtenerTodoConCondicion(e => e.FicEstadoRegistro == "activo" && e.FicCodigo == fichaNew.FicCodigo,
                                                                    e => e.Include(c => c.programaFormacion)
                                                                            .ThenInclude(c => c.Area)
                                                                                .ThenInclude(c => c.AreaPsicologo)
                                                                            .Include(c => c.programaFormacion.Centro)
                                                                                .ThenInclude(c => c.Regional)
                                                                            .Include(c => c.programaFormacion.NivelFormacion));

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró el registro!");
            }

            return Ok(new
            {
                mensaje = "Ficha creada correctamente.",
                datos
            });
        }

        [HttpPut("editar/{id}")]
        public async Task<IActionResult> EditarFicha(int id, [FromBody] FichaDTO fichaRecibida)
        {
            if (id.ToString() == "")
            {
                return BadRequest("El ID no debe ser nulo");
            }
            var fichaEncontrada = await _uow.Ficha.ObtenerPorID(id);
            
            if (fichaEncontrada == null)
            {
                return NotFound("No se encontró este ID");
            }

            fichaEncontrada.FicCodigo = fichaRecibida.FicCodigo;
            fichaEncontrada.FicJornada = fichaRecibida.FicJornada;
            fichaEncontrada.FicFechaInicio = fichaRecibida.FicFechaInicio;
            fichaEncontrada.FicFechaFin = fichaRecibida.FicFechaFin;
            fichaEncontrada.FicEstadoFormacion = fichaRecibida.FicEstadoFormacion;
            fichaEncontrada.FicProgramaFk = fichaRecibida.FicProgramaFk;



            _uow.Ficha.Actualizar(fichaEncontrada);
            await _uow.SaveChangesAsync();

            var datos = await _uow.Ficha.ObtenerTodoConCondicion(e => e.FicEstadoRegistro == "activo" && e.FicCodigo == fichaEncontrada.FicCodigo,
                                                                    e => e.Include(c => c.programaFormacion)
                                                                            .ThenInclude(c => c.Area)
                                                                                .ThenInclude(c => c.AreaPsicologo)
                                                                            .Include(c => c.programaFormacion.Centro)
                                                                                .ThenInclude(c => c.Regional)
                                                                            .Include(c => c.programaFormacion.NivelFormacion));

            return Ok(new
            {
                mensaje = "Ficha editada correctamente.",
                datos
            });
        }



        [HttpPut("cambiar-estado/{id}")]
        public async Task<IActionResult> EliminarFicha(int id)
        {
            var fichaEncontrada = await _uow.Ficha.ObtenerPorID(id);
            var estadoActual = fichaEncontrada.FicEstadoRegistro?.ToLower();

            if (estadoActual == "activo")
            {
                fichaEncontrada.FicEstadoRegistro = "inactivo";            
            } else
            {
                fichaEncontrada.FicEstadoRegistro = "activo";            
            }

            _uow.Ficha.Actualizar(fichaEncontrada);
            await _uow.SaveChangesAsync();

            return Ok("Se ha cambiado el estado correctamente!");
        }


        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarDefinitivo(int id)
        {
            var aprEncontrado = await _uow.Ficha.ObtenerPorID(id);
            if (aprEncontrado == null)
            {
                return NotFound("No se encontró este ID");
            }

            _uow.Ficha.Eliminar(aprEncontrado);
            await _uow.SaveChangesAsync();
            return Ok("Se ha eliminado correctamente!");
        }
    }
}
