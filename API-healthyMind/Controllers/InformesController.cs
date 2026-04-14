using API_healthyMind.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_healthyMind.Controllers
{
    /// <summary>
    /// Estadísticas agregadas para el panel administrador.
    /// Todos los endpoints aceptan filtros opcionales <c>desde</c> y <c>hasta</c> (yyyy-MM-dd).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "SoloAdministrador")]
    public class InformesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public InformesController(IUnidadDeTrabajo uow)
        {
            _db = uow.ObtenerContexto();
        }

        // ───────── Helpers ─────────

        private (DateTime? desde, DateTime? hasta) ParseRangoFechas(string? desdeStr, string? hastaStr)
        {
            DateTime? desde = null, hasta = null;
            if (DateTime.TryParse(desdeStr, out var d)) desde = d.Date;
            if (DateTime.TryParse(hastaStr, out var h)) hasta = h.Date.AddDays(1);
            return (desde, hasta);
        }

        // ═══════════════════════════════════════════
        //  CITAS — rankings territoriales y por psicólogo
        // ═══════════════════════════════════════════

        /// <summary>Ranking de cantidad de citas por centro.</summary>
        [HttpGet("citas-por-centro")]
        public async Task<IActionResult> CitasPorCentro(
            [FromQuery] string? desde = null, [FromQuery] string? hasta = null)
        {
            var (d, h) = ParseRangoFechas(desde, hasta);

            var query = _db.Citas
                .Where(c => c.CitEstadoRegistro == "activo")
                .Include(c => c.CitAprCodFkNavigation)
                    .ThenInclude(af => af!.Ficha)
                        .ThenInclude(f => f!.programaFormacion)
                            .ThenInclude(p => p!.Centro)
                .AsQueryable();

            if (d.HasValue) query = query.Where(c => c.CitFechaCreacion >= d.Value);
            if (h.HasValue) query = query.Where(c => c.CitFechaCreacion < h.Value);

            var datos = await query.ToListAsync();

            var resultado = datos
                .Where(c => c.CitAprCodFkNavigation?.Ficha?.programaFormacion?.Centro != null)
                .GroupBy(c => new
                {
                    CenCodigo = c.CitAprCodFkNavigation!.Ficha!.programaFormacion!.Centro!.CenCodigo,
                    CenNombre = c.CitAprCodFkNavigation!.Ficha!.programaFormacion!.Centro!.CenNombre
                })
                .Select(g => new
                {
                    centro = g.Key.CenNombre,
                    centroCodigo = g.Key.CenCodigo,
                    totalCitas = g.Count()
                })
                .OrderByDescending(x => x.totalCitas)
                .ToList();

            return Ok(resultado);
        }

        /// <summary>Ranking de cantidad de citas por regional.</summary>
        [HttpGet("citas-por-regional")]
        public async Task<IActionResult> CitasPorRegional(
            [FromQuery] string? desde = null, [FromQuery] string? hasta = null)
        {
            var (d, h) = ParseRangoFechas(desde, hasta);

            var query = _db.Citas
                .Where(c => c.CitEstadoRegistro == "activo")
                .Include(c => c.CitAprCodFkNavigation)
                    .ThenInclude(af => af!.Ficha)
                        .ThenInclude(f => f!.programaFormacion)
                            .ThenInclude(p => p!.Centro)
                                .ThenInclude(ce => ce!.Regional)
                .AsQueryable();

            if (d.HasValue) query = query.Where(c => c.CitFechaCreacion >= d.Value);
            if (h.HasValue) query = query.Where(c => c.CitFechaCreacion < h.Value);

            var datos = await query.ToListAsync();

            var resultado = datos
                .Where(c => c.CitAprCodFkNavigation?.Ficha?.programaFormacion?.Centro?.Regional != null)
                .GroupBy(c => new
                {
                    RegCodigo = c.CitAprCodFkNavigation!.Ficha!.programaFormacion!.Centro!.Regional!.RegCodigo,
                    RegNombre = c.CitAprCodFkNavigation!.Ficha!.programaFormacion!.Centro!.Regional!.RegNombre
                })
                .Select(g => new
                {
                    regional = g.Key.RegNombre,
                    regionalCodigo = g.Key.RegCodigo,
                    totalCitas = g.Count()
                })
                .OrderByDescending(x => x.totalCitas)
                .ToList();

            return Ok(resultado);
        }

        /// <summary>Tasa de asistencia y cancelación de citas por centro.</summary>
        [HttpGet("tasa-asistencia-por-centro")]
        public async Task<IActionResult> TasaAsistenciaPorCentro(
            [FromQuery] string? desde = null, [FromQuery] string? hasta = null)
        {
            var (d, h) = ParseRangoFechas(desde, hasta);

            var query = _db.Citas
                .Where(c => c.CitEstadoRegistro == "activo")
                .Include(c => c.CitAprCodFkNavigation)
                    .ThenInclude(af => af!.Ficha)
                        .ThenInclude(f => f!.programaFormacion)
                            .ThenInclude(p => p!.Centro)
                .AsQueryable();

            if (d.HasValue) query = query.Where(c => c.CitFechaCreacion >= d.Value);
            if (h.HasValue) query = query.Where(c => c.CitFechaCreacion < h.Value);

            var datos = await query.ToListAsync();

            var resultado = datos
                .Where(c => c.CitAprCodFkNavigation?.Ficha?.programaFormacion?.Centro != null)
                .GroupBy(c => c.CitAprCodFkNavigation!.Ficha!.programaFormacion!.Centro!.CenNombre)
                .Select(g =>
                {
                    var total = g.Count();
                    var realizadas = g.Count(c => c.CitEstadoCita?.ToLower() == "realizada");
                    var canceladas = g.Count(c => c.CitEstadoCita?.ToLower() == "cancelada");
                    var noAsistidas = g.Count(c => c.CitEstadoCita?.ToLower() == "no asistió");
                    return new
                    {
                        centro = g.Key,
                        total,
                        realizadas,
                        canceladas,
                        noAsistidas,
                        tasaAsistenciaPct = total > 0 ? Math.Round((double)realizadas / total * 100, 1) : 0,
                        tasaCancelacionPct = total > 0 ? Math.Round((double)(canceladas + noAsistidas) / total * 100, 1) : 0
                    };
                })
                .OrderByDescending(x => x.total)
                .ToList();

            return Ok(resultado);
        }

        /// <summary>Ranking de citas por psicólogo (carga de trabajo).</summary>
        [HttpGet("citas-por-psicologo")]
        public async Task<IActionResult> CitasPorPsicologo(
            [FromQuery] string? desde = null, [FromQuery] string? hasta = null)
        {
            var (d, h) = ParseRangoFechas(desde, hasta);

            var query = _db.Citas
                .Where(c => c.CitEstadoRegistro == "activo")
                .Include(c => c.CitPsiCodFkNavigation)
                .AsQueryable();

            if (d.HasValue) query = query.Where(c => c.CitFechaCreacion >= d.Value);
            if (h.HasValue) query = query.Where(c => c.CitFechaCreacion < h.Value);

            var datos = await query.ToListAsync();

            var resultado = datos
                .Where(c => c.CitPsiCodFkNavigation != null)
                .GroupBy(c => new
                {
                    c.CitPsiCodFkNavigation!.PsiCodigo,
                    c.CitPsiCodFkNavigation.PsiNombre,
                    c.CitPsiCodFkNavigation.PsiApellido
                })
                .Select(g =>
                {
                    var total = g.Count();
                    var realizadas = g.Count(c => c.CitEstadoCita?.ToLower() == "realizada");
                    return new
                    {
                        psicologo = $"{g.Key.PsiNombre} {g.Key.PsiApellido}".Trim(),
                        psicologoCodigo = g.Key.PsiCodigo,
                        total,
                        realizadas,
                        pendientes = g.Count(c => c.CitEstadoCita?.ToLower() == "pendiente"),
                        canceladas = g.Count(c => c.CitEstadoCita?.ToLower() == "cancelada"),
                        tasaEfectividadPct = total > 0 ? Math.Round((double)realizadas / total * 100, 1) : 0
                    };
                })
                .OrderByDescending(x => x.total)
                .ToList();

            return Ok(resultado);
        }

        // ═══════════════════════════════════════════
        //  APRENDICES — distribución territorial y académica
        // ═══════════════════════════════════════════

        /// <summary>Aprendices activos agrupados por regional (vía ciudad).</summary>
        [HttpGet("aprendices-por-regional")]
        public async Task<IActionResult> AprendicesPorRegional()
        {
            var datos = await _db.Aprendizs
                .Where(a => a.AprEstadoRegistro == "activo")
                .Include(a => a.Municipio)
                    .ThenInclude(c => c!.Regional)
                .ToListAsync();

            var resultado = datos
                .Where(a => a.Municipio?.Regional != null)
                .GroupBy(a => a.Municipio!.Regional!.RegNombre)
                .Select(g => new { regional = g.Key, totalAprendices = g.Count() })
                .OrderByDescending(x => x.totalAprendices)
                .ToList();

            return Ok(resultado);
        }

        /// <summary>Aprendices por programa de formación.</summary>
        [HttpGet("aprendices-por-programa")]
        public async Task<IActionResult> AprendicesPorPrograma()
        {
            var datos = await _db.AprendizFichas
                .Where(af => af.AprFicEstadoRegistro == "activo")
                .Include(af => af.Ficha)
                    .ThenInclude(f => f!.programaFormacion)
                .ToListAsync();

            var resultado = datos
                .Where(af => af.Ficha?.programaFormacion != null)
                .GroupBy(af => af.Ficha!.programaFormacion!.ProgNombre)
                .Select(g => new { programa = g.Key, totalAprendices = g.Count() })
                .OrderByDescending(x => x.totalAprendices)
                .ToList();

            return Ok(resultado);
        }

        /// <summary>Distribución de aprendices por área de conocimiento.</summary>
        [HttpGet("aprendices-por-area")]
        public async Task<IActionResult> AprendicesPorArea()
        {
            var datos = await _db.AprendizFichas
                .Where(af => af.AprFicEstadoRegistro == "activo")
                .Include(af => af.Ficha)
                    .ThenInclude(f => f!.programaFormacion)
                        .ThenInclude(p => p!.Area)
                .ToListAsync();

            var resultado = datos
                .Where(af => af.Ficha?.programaFormacion?.Area != null)
                .GroupBy(af => af.Ficha!.programaFormacion!.Area!.AreaNombre)
                .Select(g => new { area = g.Key, totalAprendices = g.Count() })
                .OrderByDescending(x => x.totalAprendices)
                .ToList();

            return Ok(resultado);
        }

        /// <summary>Distribución de aprendices por nivel de formación.</summary>
        [HttpGet("aprendices-por-nivel")]
        public async Task<IActionResult> AprendicesPorNivel()
        {
            var datos = await _db.AprendizFichas
                .Where(af => af.AprFicEstadoRegistro == "activo")
                .Include(af => af.Ficha)
                    .ThenInclude(f => f!.programaFormacion)
                        .ThenInclude(p => p!.NivelFormacion)
                .ToListAsync();

            var resultado = datos
                .Where(af => af.Ficha?.programaFormacion?.NivelFormacion != null)
                .GroupBy(af => af.Ficha!.programaFormacion!.NivelFormacion!.NivForNombre)
                .Select(g => new { nivel = g.Key, totalAprendices = g.Count() })
                .OrderByDescending(x => x.totalAprendices)
                .ToList();

            return Ok(resultado);
        }

        /// <summary>Programas de formación por centro (conteo de fichas activas).</summary>
        [HttpGet("programas-por-centro")]
        public async Task<IActionResult> ProgramasPorCentro()
        {
            var datos = await _db.Programaformacions
                .Where(p => p.ProgEstadoRegistro == "activo")
                .Include(p => p.Centro)
                .Include(p => p.Fichas)
                .ToListAsync();

            var resultado = datos
                .Where(p => p.Centro != null)
                .GroupBy(p => p.Centro!.CenNombre)
                .Select(g => new
                {
                    centro = g.Key,
                    totalProgramas = g.Count(),
                    totalFichas = g.Sum(p => p.Fichas.Count(f => f.FicEstadoRegistro == "activo"))
                })
                .OrderByDescending(x => x.totalProgramas)
                .ToList();

            return Ok(resultado);
        }

        // ═══════════════════════════════════════════
        //  SEGUIMIENTOS — estados y distribución
        // ═══════════════════════════════════════════

        /// <summary>Seguimientos agrupados por estado (crítico, estable, etc.).</summary>
        [HttpGet("seguimientos-por-estado")]
        public async Task<IActionResult> SeguimientosPorEstado(
            [FromQuery] string? desde = null, [FromQuery] string? hasta = null)
        {
            var (d, h) = ParseRangoFechas(desde, hasta);

            var query = _db.SeguimientoAprendizs
                .Where(s => s.SegEstadoRegistro == "activo")
                .AsQueryable();

            if (d.HasValue) query = query.Where(s => s.SegFechaSeguimiento >= d.Value);
            if (h.HasValue) query = query.Where(s => s.SegFechaSeguimiento < h.Value);

            var resultado = await query
                .GroupBy(s => s.SegEstadoSeguimiento ?? "Sin clasificar")
                .Select(g => new { estado = g.Key, total = g.Count() })
                .OrderByDescending(x => x.total)
                .ToListAsync();

            return Ok(resultado);
        }

        /// <summary>Seguimientos agrupados por centro.</summary>
        [HttpGet("seguimientos-por-centro")]
        public async Task<IActionResult> SeguimientosPorCentro(
            [FromQuery] string? desde = null, [FromQuery] string? hasta = null)
        {
            var (d, h) = ParseRangoFechas(desde, hasta);

            var query = _db.SeguimientoAprendizs
                .Where(s => s.SegEstadoRegistro == "activo")
                .Include(s => s.SegAprendizFkNavigation)
                    .ThenInclude(af => af!.Ficha)
                        .ThenInclude(f => f!.programaFormacion)
                            .ThenInclude(p => p!.Centro)
                .AsQueryable();

            if (d.HasValue) query = query.Where(s => s.SegFechaSeguimiento >= d.Value);
            if (h.HasValue) query = query.Where(s => s.SegFechaSeguimiento < h.Value);

            var datos = await query.ToListAsync();

            var resultado = datos
                .Where(s => s.SegAprendizFkNavigation?.Ficha?.programaFormacion?.Centro != null)
                .GroupBy(s => s.SegAprendizFkNavigation!.Ficha!.programaFormacion!.Centro!.CenNombre)
                .Select(g => new { centro = g.Key, totalSeguimientos = g.Count() })
                .OrderByDescending(x => x.totalSeguimientos)
                .ToList();

            return Ok(resultado);
        }

        /// <summary>Seguimientos agrupados por programa de formación.</summary>
        [HttpGet("seguimientos-por-programa")]
        public async Task<IActionResult> SeguimientosPorPrograma(
            [FromQuery] string? desde = null, [FromQuery] string? hasta = null)
        {
            var (d, h) = ParseRangoFechas(desde, hasta);

            var query = _db.SeguimientoAprendizs
                .Where(s => s.SegEstadoRegistro == "activo")
                .Include(s => s.SegAprendizFkNavigation)
                    .ThenInclude(af => af!.Ficha)
                        .ThenInclude(f => f!.programaFormacion)
                .AsQueryable();

            if (d.HasValue) query = query.Where(s => s.SegFechaSeguimiento >= d.Value);
            if (h.HasValue) query = query.Where(s => s.SegFechaSeguimiento < h.Value);

            var datos = await query.ToListAsync();

            var resultado = datos
                .Where(s => s.SegAprendizFkNavigation?.Ficha?.programaFormacion != null)
                .GroupBy(s => s.SegAprendizFkNavigation!.Ficha!.programaFormacion!.ProgNombre)
                .Select(g => new { programa = g.Key, totalSeguimientos = g.Count() })
                .OrderByDescending(x => x.totalSeguimientos)
                .ToList();

            return Ok(resultado);
        }

        // ═══════════════════════════════════════════
        //  TESTS — aplicación por programa
        // ═══════════════════════════════════════════

        /// <summary>Tests psicológicos aplicados, agrupados por programa de formación.</summary>
        [HttpGet("tests-por-programa")]
        public async Task<IActionResult> TestsPorPrograma(
            [FromQuery] string? desde = null, [FromQuery] string? hasta = null)
        {
            var (d, h) = ParseRangoFechas(desde, hasta);

            var query = _db.TestGenerals
                .Where(t => t.TestGenEstado == "activo")
                .Include(t => t.TestGenApreFkNavigation)
                    .ThenInclude(af => af!.Ficha)
                        .ThenInclude(f => f!.programaFormacion)
                .AsQueryable();

            if (d.HasValue) query = query.Where(t => t.TestGenFechaRealiz >= d.Value);
            if (h.HasValue) query = query.Where(t => t.TestGenFechaRealiz < h.Value);

            var datos = await query.ToListAsync();

            var resultado = datos
                .Where(t => t.TestGenApreFkNavigation?.Ficha?.programaFormacion != null)
                .GroupBy(t => t.TestGenApreFkNavigation!.Ficha!.programaFormacion!.ProgNombre)
                .Select(g =>
                {
                    var total = g.Count();
                    var completados = g.Count(t => t.TestGenEstadoTest == "completado");
                    return new
                    {
                        programa = g.Key,
                        total,
                        completados,
                        enProgreso = g.Count(t => t.TestGenEstadoTest == "en_progreso"),
                        asignados = g.Count(t => t.TestGenEstadoTest == "asignado"),
                        tasaCompletadoPct = total > 0 ? Math.Round((double)completados / total * 100, 1) : 0
                    };
                })
                .OrderByDescending(x => x.total)
                .ToList();

            return Ok(resultado);
        }

        // ═══════════════════════════════════════════
        //  REPORTES / INCIDENCIAS — evolución
        // ═══════════════════════════════════════════

        /// <summary>Reportes agrupados por mes y estado en un rango de fechas.</summary>
        [HttpGet("reportes-por-periodo")]
        public async Task<IActionResult> ReportesPorPeriodo(
            [FromQuery] string? desde = null, [FromQuery] string? hasta = null)
        {
            var (d, h) = ParseRangoFechas(desde, hasta);

            var query = _db.Reportes
                .Where(r => r.RepEstadoRegistro == "activo")
                .AsQueryable();

            if (d.HasValue) query = query.Where(r => r.RepFechaCreacion >= d.Value);
            if (h.HasValue) query = query.Where(r => r.RepFechaCreacion < h.Value);

            var resultado = await query
                .GroupBy(r => new { Año = r.RepFechaCreacion.Year, Mes = r.RepFechaCreacion.Month })
                .Select(g => new
                {
                    anio = g.Key.Año,
                    mes = g.Key.Mes,
                    total = g.Count(),
                    creados = g.Count(r => r.RepEstado == "creado"),
                    enProceso = g.Count(r => r.RepEstado == "en_proceso" || r.RepEstado == "proceso"),
                    resueltos = g.Count(r => r.RepEstado == "resuelto"),
                    cancelados = g.Count(r => r.RepEstado == "cancelado")
                })
                .OrderBy(x => x.anio).ThenBy(x => x.mes)
                .ToListAsync();

            return Ok(resultado);
        }

        // ═══════════════════════════════════════════
        //  RESUMEN GENERAL (snapshot para reuniones)
        // ═══════════════════════════════════════════

        /// <summary>Resumen general: totales globales de todas las entidades relevantes.</summary>
        [HttpGet("resumen-general")]
        public async Task<IActionResult> ResumenGeneral()
        {
            var aprendicesActivos = await _db.Aprendizs.CountAsync(a => a.AprEstadoRegistro == "activo");
            var psicologosActivos = await _db.Psicologos.CountAsync(p => p.PsiEstadoRegistro == "activo");
            var citasTotal = await _db.Citas.CountAsync(c => c.CitEstadoRegistro == "activo");
            var citasRealizadas = await _db.Citas.CountAsync(c => c.CitEstadoRegistro == "activo" && c.CitEstadoCita == "realizada");
            var seguimientosActivos = await _db.SeguimientoAprendizs.CountAsync(s => s.SegEstadoRegistro == "activo");
            var testsCompletados = await _db.TestGenerals.CountAsync(t => t.TestGenEstado == "activo" && t.TestGenEstadoTest == "completado");
            var reportesPendientes = await _db.Reportes.CountAsync(r => r.RepEstadoRegistro == "activo" && r.RepEstado == "creado");
            var centrosActivos = await _db.Centros.CountAsync(c => c.CenEstadoRegistro == "activo");
            var fichasActivas = await _db.Fichas.CountAsync(f => f.FicEstadoRegistro == "activo");

            return Ok(new
            {
                aprendicesActivos,
                psicologosActivos,
                citasTotal,
                citasRealizadas,
                tasaEfectividadCitasPct = citasTotal > 0 ? Math.Round((double)citasRealizadas / citasTotal * 100, 1) : 0,
                seguimientosActivos,
                testsCompletados,
                reportesPendientes,
                centrosActivos,
                fichasActivas
            });
        }
    }
}
