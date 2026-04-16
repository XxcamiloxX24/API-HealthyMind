using API_healthyMind.Data;
using API_healthyMind.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace API_healthyMind.Services;

public class EmotionalStreakEvaluator : IEmotionalStreakEvaluator
{
    private readonly AppDbContext _db;
    private readonly IChatPushService _chatPush;
    private readonly ILogger<EmotionalStreakEvaluator> _logger;
    private const int RachaMinima = 3;
    private const int UmbralEscala = 5;

    public EmotionalStreakEvaluator(
        AppDbContext db,
        IChatPushService chatPush,
        ILogger<EmotionalStreakEvaluator> logger)
    {
        _db = db;
        _chatPush = chatPush;
        _logger = logger;
    }

    private static string CategoriaDeEscala(int escala) => escala switch
    {
        <= 2 => "Critica",
        <= 4 => "Negativa",
        <= 6 => "Neutral",
        _ => "Positiva"
    };

    public async Task EvaluarYNotificarAsync(int aprendizId, CancellationToken ct = default)
    {
        try
        {
            var diasRiesgo = await ObtenerUltimosDiasConRiesgoAsync(aprendizId, ct);
            if (diasRiesgo == null || diasRiesgo.Count < RachaMinima)
                return;

            var seguimientos = await ObtenerSeguimientosActivosAsync(aprendizId, ct);
            if (seguimientos.Count == 0)
            {
                _logger.LogDebug(
                    "Racha emocional detectada para aprendiz {Apr} pero no tiene seguimientos activos.",
                    aprendizId);
                return;
            }

            var fechaReciente = diasRiesgo[0].Fecha;
            var fechasIso = diasRiesgo.Select(d => d.Fecha.ToString("yyyy-MM-dd")).ToList();
            var escalas = diasRiesgo.Select(d => d.PromedioEscala).ToList();
            var regla = DeterminarRegla(diasRiesgo);

            foreach (var seg in seguimientos)
            {
                if (seg.SegPsicologoFk == null) continue;
                var psiId = seg.SegPsicologoFk.Value;

                var yaExiste = await _db.AlertasRachaEmocional.AnyAsync(a =>
                    a.AreAprendizFk == aprendizId &&
                    a.ArePsicologoFk == psiId &&
                    a.AreFechaReciente == fechaReciente &&
                    a.AreEstadoRegistro == "activo", ct);

                if (yaExiste)
                {
                    _logger.LogDebug(
                        "Alerta racha ya existe: aprendiz={Apr}, psi={Psi}, fecha={Fecha}",
                        aprendizId, psiId, fechaReciente);
                    continue;
                }

                var nombreAprendiz = await ObtenerNombreAprendizAsync(aprendizId, ct);
                var mensaje = $"{nombreAprendiz} tiene {RachaMinima} registros recientes con emociones de riesgo ({string.Join(", ", fechasIso.Select(f => FormatearFechaCorta(f)))})";

                var alerta = new AlertaRachaEmocional
                {
                    AreAprendizFk = aprendizId,
                    ArePsicologoFk = psiId,
                    AreSeguimientoFk = seg.SegCodigo,
                    AreFechaReciente = fechaReciente,
                    AreRegla = regla,
                    AreFechasJson = JsonSerializer.Serialize(fechasIso),
                    AreEscalasJson = JsonSerializer.Serialize(escalas),
                    AreMensaje = mensaje,
                    AreEstado = "nueva",
                    AreFechaCreacion = DateTime.Now,
                    AreEstadoRegistro = "activo"
                };

                _db.AlertasRachaEmocional.Add(alerta);

                try
                {
                    await _db.SaveChangesAsync(ct);
                }
                catch (DbUpdateException ex) when (EsDuplicateKey(ex))
                {
                    _logger.LogDebug(
                        "Alerta racha duplicada (concurrent insert): aprendiz={Apr}, psi={Psi}",
                        aprendizId, psiId);
                    _db.Entry(alerta).State = EntityState.Detached;
                    continue;
                }

                _logger.LogInformation(
                    "Alerta racha emocional creada: aprendiz={Apr}, psi={Psi}, fechas={Fechas}",
                    aprendizId, psiId, string.Join(",", fechasIso));

                await _chatPush.NotifyPsychologistAsync(
                    psiId,
                    "RACHA_EMOCIONAL",
                    "Alerta emocional",
                    mensaje,
                    appointmentId: null,
                    seguimientoId: seg.SegCodigo,
                    deepLink: "followups",
                    cancellationToken: ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Error evaluando racha emocional para aprendiz {Apr}. No se interrumpe el flujo principal.",
                aprendizId);
        }
    }

    private async Task<List<DiaRiesgo>?> ObtenerUltimosDiasConRiesgoAsync(int aprendizId, CancellationToken ct)
    {
        var paginas = await _db.Set<PaginaDiario>()
            .AsNoTracking()
            .Where(p =>
                p.PagEstadoRegistro == "activo" &&
                p.PagDiarioFkNavigation != null &&
                p.PagDiarioFkNavigation.DiaEstadoRegistro == "activo" &&
                p.PagDiarioFkNavigation.DiaAprendizFk == aprendizId &&
                p.PagEmocionFk != null)
            .Include(p => p.PagEmocionFkNavigation)
            .ToListAsync(ct);

        var diasAgrupados = paginas
            .GroupBy(p => p.PagFechaRealizacion.Date)
            .OrderByDescending(g => g.Key)
            .Take(RachaMinima)
            .Select(g =>
            {
                var conEmocion = g.Where(p => p.PagEmocionFkNavigation != null).ToList();
                var promedio = conEmocion.Any()
                    ? Math.Round(conEmocion.Average(p => p.PagEmocionFkNavigation!.EmoEscala), 1)
                    : 0;
                var escalaRedondeada = (int)Math.Round(promedio);
                var categoria = CategoriaDeEscala(escalaRedondeada);
                var cumpleCategoria = categoria is "Critica" or "Negativa";
                var cumpleEscala = escalaRedondeada <= UmbralEscala;

                return new DiaRiesgo
                {
                    Fecha = DateOnly.FromDateTime(g.Key),
                    PromedioEscala = promedio,
                    Categoria = categoria,
                    Cumple = cumpleCategoria || cumpleEscala
                };
            })
            .ToList();

        if (diasAgrupados.Count < RachaMinima)
            return null;

        return diasAgrupados.All(d => d.Cumple) ? diasAgrupados : null;
    }

    private async Task<List<SeguimientoAprendiz>> ObtenerSeguimientosActivosAsync(int aprendizId, CancellationToken ct)
    {
        return await _db.Set<SeguimientoAprendiz>()
            .AsNoTracking()
            .Where(s =>
                s.SegEstadoRegistro == "activo" &&
                s.SegPsicologoFk != null &&
                s.SegEstadoSeguimiento != EstadosSeguimiento.Completada &&
                s.SegAprendizFkNavigation != null &&
                s.SegAprendizFkNavigation.AprFicAprendizFk == aprendizId)
            .ToListAsync(ct);
    }

    private async Task<string> ObtenerNombreAprendizAsync(int aprendizId, CancellationToken ct)
    {
        var apr = await _db.Set<Aprendiz>()
            .AsNoTracking()
            .Where(a => a.AprCodigo == aprendizId)
            .Select(a => new { a.AprNombre, a.AprApellido })
            .FirstOrDefaultAsync(ct);

        return apr != null ? $"{apr.AprNombre} {apr.AprApellido}".Trim() : $"Aprendiz #{aprendizId}";
    }

    private static string DeterminarRegla(List<DiaRiesgo> dias)
    {
        bool todasCategoria = dias.All(d => d.Categoria is "Critica" or "Negativa");
        bool todasEscala = dias.All(d => (int)Math.Round(d.PromedioEscala) <= UmbralEscala);

        if (todasCategoria && todasEscala) return "AMBAS";
        if (todasCategoria) return "CATEGORIA_NEG_CRIT";
        return "ESCALA_LE_5";
    }

    private static string FormatearFechaCorta(string iso)
    {
        if (DateOnly.TryParse(iso, out var d))
            return d.ToString("dd MMM", new System.Globalization.CultureInfo("es-CO"));
        return iso;
    }

    private static bool EsDuplicateKey(DbUpdateException ex)
    {
        var inner = ex.InnerException?.Message ?? ex.Message;
        return inner.Contains("Duplicate entry", StringComparison.OrdinalIgnoreCase)
            || inner.Contains("uq_alerta_idempotencia", StringComparison.OrdinalIgnoreCase);
    }

    private class DiaRiesgo
    {
        public DateOnly Fecha { get; set; }
        public double PromedioEscala { get; set; }
        public string Categoria { get; set; } = "";
        public bool Cumple { get; set; }
    }
}
