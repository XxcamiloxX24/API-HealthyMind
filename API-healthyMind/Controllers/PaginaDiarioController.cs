using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Models.DTO.Filtros;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CualquierRol")]
    public class PaginaDiarioController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;
        


        public PaginaDiarioController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
            
        }

        private static object MapearAprendiz(Aprendiz d)
        {
            return new
            {
                Codigo = d.AprCodigo,
                FechaCreacion = d.AprFechaCreacion,
                TipoDocumento = d.AprTipoDocumento,
                NroDocumento = d.AprNroDocumento,
                FechaNacimiento = d.AprFechaNac,
                Nombres = new
                {
                    PrimerNombre = d.AprNombre,
                    SegundoNombre = d.AprSegundoNombre
                },
                Apellidos = new
                {
                    PrimerApellido = d.AprApellido,
                    SegundoApellido = d.AprSegundoApellido
                },
                Ubicacion = d.Municipio == null ? null : new
                {
                    DepartamentoID = d.Municipio.Regional.RegCodigo,
                    Departamento = d.Municipio.Regional.RegNombre,
                    MunicipioID = d.Municipio.CiuCodigo,
                    Municipio = d.Municipio.CiuNombre,
                    Direccion = d.AprDireccion
                },
                Contacto = new
                {
                    Telefono = d.AprTelefono,
                    CorreoInstitucional = d.AprCorreoInstitucional,
                    CorreoPersonal = d.AprCorreoPersonal,
                    Acudiente = new
                    {
                        AcudienteNombre = d.AprAcudNombre,
                        AcudienteApellido = d.AprAcudApellido,
                        AcudienteTelefono = d.AprTelefonoAcudiente
                    }
                },
                d.EstadoAprendiz,
                Eps = d.AprEps,
                Patologia = d.AprPatologia,
                TipoPoblacion = d.AprTipoPoblacion
            };
        }

        private static object MapearAprendizDiario(Diario d)
        {
            return new
            {
                d.DiaCodigo,
                d.DiaTitulo,
                d.DiaImagenUrl,
                d.DiaFechaCreacion,
                Aprendiz = MapearAprendiz(d.aprendiz)
            };
        }

        private static object MapearPaginaDiario(PaginaDiario d)
        {
            return new
            {
                d.PagCodigo,
                d.PagTitulo,
                d.PagContenido,
                d.PagImagenUrl,
                d.PagFechaRealizacion,
                diario = d.PagDiarioFkNavigation != null ? MapearAprendizDiario(d.PagDiarioFkNavigation) : null,
                emociones = d.PagEmocionFkNavigation,
            };
        }

        // GET: NivelFormacionController
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.PaginaDiario.ObtenerTodoConCondicion(e => e.PagEstadoRegistro == "activo" || e.PagEstadoRegistro == "inactivo",
                e => e
                    .Include(c => c.PagDiarioFkNavigation)
                        .ThenInclude(c => c.aprendiz)
                            .ThenInclude(c => c.Municipio)
                                .ThenInclude(c => c.Regional)
                    .Include(c => c.PagDiarioFkNavigation.aprendiz.EstadoAprendiz)
                    .Include(c => c.PagEmocionFkNavigation));
                            
                       

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            var resultados = datos.Select(c => MapearPaginaDiario(c));

            return Ok(resultados);
        }

        [HttpGet("activos")]
        public async Task<IActionResult> ObtenerTodosActivos()
        {
            var datos = await _uow.PaginaDiario.ObtenerTodoConCondicion(e => e.PagEstadoRegistro == "activo",
                e => e
                    .Include(c => c.PagDiarioFkNavigation)
                        .ThenInclude(c => c.aprendiz)
                            .ThenInclude(c => c.Municipio)
                                .ThenInclude(c => c.Regional)
                    .Include(c => c.PagDiarioFkNavigation.aprendiz.EstadoAprendiz)
                    .Include(c => c.PagEmocionFkNavigation));



            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            var resultados = datos.Select(c => MapearPaginaDiario(c));

            return Ok(resultados);
        }

        /// <summary>
        /// Todas las páginas activas de un diario (contenido + emoción), ordenadas por fecha descendente.
        /// </summary>
        [HttpGet("diario/{diarioId:int}")]
        public async Task<IActionResult> ObtenerPaginasPorDiario(int diarioId)
        {
            var existeDiario = await _uow.Diario.Query()
                .AsNoTracking()
                .AnyAsync(d => d.DiaCodigo == diarioId && d.DiaEstadoRegistro != null && d.DiaEstadoRegistro.ToLower() == "activo");

            if (!existeDiario)
                return NotFound("No existe un diario activo con ese código.");

            var datos = await _uow.PaginaDiario.Query()
                .AsNoTracking()
                .Where(x =>
                    x.PagDiarioFk == diarioId &&
                    x.PagEstadoRegistro != null &&
                    x.PagEstadoRegistro.ToLower() == "activo")
                .Include(x => x.PagEmocionFkNavigation)
                .OrderByDescending(x => x.PagFechaRealizacion)
                .ThenByDescending(x => x.PagCodigo)
                .ToListAsync();

            var paginas = datos.Select(p => new
            {
                p.PagCodigo,
                p.PagTitulo,
                p.PagContenido,
                p.PagFechaRealizacion,
                emocion = p.PagEmocionFkNavigation
            });

            return Ok(new
            {
                diarioId,
                totalPaginas = datos.Count,
                paginas
            });
        }

        /// <summary>
        /// Todas las páginas activas de un día concreto del diario (sin paginar por índice).
        /// Sin <paramref name="fecha"/>: día más reciente que tenga entradas.
        /// Para otro día, enviar <paramref name="fecha"/>; use <c>fechaMasNuevaConEntradas</c> / <c>fechaMasAntiguaConEntradas</c> para navegar.
        /// </summary>
        [HttpGet("paginacion-por-fecha")]
        public async Task<IActionResult> ObtenerPaginacionPorFecha(int diarioId, DateOnly? fecha = null)
        {
            var registros = await _uow.PaginaDiario.Query()
                .Where(x => x.PagEstadoRegistro == "activo" && x.PagDiarioFk == diarioId)
                .Include(x => x.PagDiarioFkNavigation)
                    .ThenInclude(d => d.aprendiz)
                .Include(x => x.PagEmocionFkNavigation)
                .ToListAsync();

            var grupos = registros
                .GroupBy(x => x.PagFechaRealizacion.Date)
                .OrderByDescending(g => g.Key)
                .ToList();

            if (!grupos.Any())
                return NotFound("No hay registros para este diario.");

            int index;
            if (fecha.HasValue)
            {
                var fechaBuscada = fecha.Value.ToDateTime(TimeOnly.MinValue);
                index = grupos.FindIndex(g => g.Key == fechaBuscada);
                if (index == -1)
                    return NotFound("No existen registros para la fecha indicada.");
            }
            else
                index = 0;

            var grupoSeleccionado = grupos[index];
            var datos = grupoSeleccionado.Select(MapearPaginaDiario).ToList();

            var fechasOrdenadas = grupos.Select(g => DateOnly.FromDateTime(g.Key)).ToList();

            return Ok(new
            {
                diarioId,
                fechaCorrespondiente = DateOnly.FromDateTime(grupoSeleccionado.Key),
                totalRegistrosEnFecha = datos.Count,
                totalDiasConEntradas = grupos.Count,
                indiceDia = index,
                fechaMasNuevaConEntradas = index > 0 ? fechasOrdenadas[index - 1] : (DateOnly?)null,
                fechaMasAntiguaConEntradas = index < grupos.Count - 1 ? fechasOrdenadas[index + 1] : (DateOnly?)null,
                fechasConEntradas = fechasOrdenadas,
                datos
            });
        }

        [HttpPost]
        public async Task<IActionResult> CrearRegistro(PaginaDiarioDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("El cuerpo no debe estar vacio!");
            }

            var nuevoReg = new PaginaDiario
            {
                PagTitulo = dto.PagTitulo,
                PagContenido = dto.PagContenido,
                PagImagenUrl = dto.PagImagenUrl,
                PagFechaRealizacion = DateTime.Now,
                PagDiarioFk = dto.PagDiarioFk,
                PagEmocionFk = dto.PagEmocionFk
            };

            await _uow.PaginaDiario.Agregar(nuevoReg);
            await _uow.SaveChangesAsync();

            var datos = await _uow.PaginaDiario.ObtenerTodoConCondicion(e => e.PagCodigo == nuevoReg.PagCodigo,
                e => e
                    .Include(c => c.PagDiarioFkNavigation)
                        .ThenInclude(c => c.aprendiz)
                            .ThenInclude(c => c.Municipio)
                                .ThenInclude(c => c.Regional)
                    .Include(c => c.PagDiarioFkNavigation.aprendiz.EstadoAprendiz)
                    .Include(c => c.PagEmocionFkNavigation));


            return Ok(new
            {
                mensaje = "Se ha creado el registro exitosamente",
                datos
            });
        }

        [HttpPut("editar/{id}")]
        public async Task<IActionResult> EditarInformacion(int id, [FromBody] PaginaDiarioDTO dto)
        {
            var diarioEncontrado = await _uow.PaginaDiario.ObtenerTodoConCondicion(a => a.PagCodigo == id 
            && a.PagDiarioFkNavigation.DiaEstadoRegistro == "activo",
            e => e
                    .Include(c => c.PagDiarioFkNavigation)
                        .ThenInclude(c => c.aprendiz)
                            .ThenInclude(c => c.Municipio)
                                .ThenInclude(c => c.Regional)
                    .Include(c => c.PagDiarioFkNavigation.aprendiz.EstadoAprendiz)
                    .Include(c => c.PagEmocionFkNavigation));

            if (!diarioEncontrado.Any())
            {
                return NotFound("No se encontró este aprendiz");
            }

            var resultado = diarioEncontrado.FirstOrDefault();

            
            resultado.PagTitulo = dto.PagTitulo;
            resultado.PagContenido = dto.PagContenido;
            resultado.PagImagenUrl = dto.PagImagenUrl;
            resultado.PagDiarioFk = dto.PagDiarioFk;
            resultado.PagEmocionFk = dto.PagEmocionFk;

            _uow.PaginaDiario.Actualizar(resultado);
            await _uow.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Se han editado correctamente los datos!",
                resultado
            });
        }


        /// <summary>
        /// Soft delete del diario (cambia estado a inactivo). Aprendiz: solo su propio diario. Admin/Psicólogo: cualquiera.
        /// </summary>
        [HttpPut("eliminar/{id}")]
        public async Task<IActionResult> EliminarDiario(int id)
        {
            var diarioEncontrado = await _uow.Diario.ObtenerTodoConCondicion(a => a.DiaCodigo == id && a.DiaEstadoRegistro == "activo",
                a => a.Include(d => d.aprendiz));

            var diario = diarioEncontrado.FirstOrDefault();

            if (diario == null)
                return NotFound("No se encontró este id.");

            if (User.IsInRole(Models.Roles.Aprendiz))
            {
                var miId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("nameid");
                if (string.IsNullOrEmpty(miId) || diario.DiaAprendizFk.ToString() != miId)
                    return StatusCode(403, "Solo puedes eliminar tu propio diario.");
            }

            diario.DiaEstadoRegistro = "inactivo";

            _uow.Diario.Actualizar(diario);
            await _uow.SaveChangesAsync();
            return Ok("Se ha eliminado correctamente ");


        }

    }
}
