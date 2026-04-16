using API_healthyMind.Data;
using API_healthyMind.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_healthyMind.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdministradorYPsicologo")]
public class AlertaRachaEmocionalController : ControllerBase
{
    private readonly IUnidadDeTrabajo _uow;

    public AlertaRachaEmocionalController(IUnidadDeTrabajo uow)
    {
        _uow = uow;
    }

    private bool TryObtenerPsicologoId(out int psiId)
    {
        psiId = 0;
        var val = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue("nameid");
        return !string.IsNullOrWhiteSpace(val) && int.TryParse(val, out psiId);
    }

    private static object Mapear(AlertaRachaEmocional a) => new
    {
        a.AreCodigo,
        a.AreAprendizFk,
        a.ArePsicologoFk,
        a.AreSeguimientoFk,
        areFechaReciente = a.AreFechaReciente.ToString("yyyy-MM-dd"),
        a.AreRegla,
        a.AreFechasJson,
        a.AreEscalasJson,
        a.AreMensaje,
        a.AreEstado,
        a.AreFechaCreacion,
        a.AreFechaLectura,
        a.AreFechaResolucion,
        a.AreNotasResolucion,
        aprendiz = a.AreAprendizFkNavigation != null ? new
        {
            a.AreAprendizFkNavigation.AprCodigo,
            a.AreAprendizFkNavigation.AprNombre,
            a.AreAprendizFkNavigation.AprApellido,
            a.AreAprendizFkNavigation.AprCorreoInstitucional
        } : null
    };

    /// <summary>Alertas activas del psicólogo autenticado, ordenadas por fecha desc.</summary>
    [HttpGet("mis-alertas")]
    public async Task<IActionResult> MisAlertas()
    {
        if (!TryObtenerPsicologoId(out var psiId))
            return Unauthorized("No se pudo identificar al psicólogo.");

        var alertas = await _uow.AlertaRachaEmocional.Query()
            .AsNoTracking()
            .Where(a => a.ArePsicologoFk == psiId && a.AreEstadoRegistro == "activo")
            .Include(a => a.AreAprendizFkNavigation)
            .OrderByDescending(a => a.AreFechaCreacion)
            .ToListAsync();

        return Ok(alertas.Select(Mapear));
    }

    /// <summary>Alertas de un aprendiz específico (para la tab Alertas en StudentProfile).</summary>
    [HttpGet("por-aprendiz/{aprendizId:int}")]
    public async Task<IActionResult> PorAprendiz(int aprendizId)
    {
        if (!TryObtenerPsicologoId(out var psiId))
            return Unauthorized("No se pudo identificar al psicólogo.");

        var alertas = await _uow.AlertaRachaEmocional.Query()
            .AsNoTracking()
            .Where(a =>
                a.AreAprendizFk == aprendizId &&
                a.ArePsicologoFk == psiId &&
                a.AreEstadoRegistro == "activo")
            .Include(a => a.AreAprendizFkNavigation)
            .OrderByDescending(a => a.AreFechaCreacion)
            .ToListAsync();

        return Ok(alertas.Select(Mapear));
    }

    /// <summary>Marcar alerta como leída.</summary>
    [HttpPut("marcar-leida/{id:int}")]
    public async Task<IActionResult> MarcarLeida(int id)
    {
        if (!TryObtenerPsicologoId(out var psiId))
            return Unauthorized();

        var alerta = await _uow.AlertaRachaEmocional.ObtenerPrimero(
            a => a.AreCodigo == id && a.ArePsicologoFk == psiId && a.AreEstadoRegistro == "activo");

        if (alerta == null)
            return NotFound("Alerta no encontrada.");

        if (alerta.AreEstado == "resuelta")
            return BadRequest("La alerta ya fue resuelta.");

        alerta.AreEstado = "leida";
        alerta.AreFechaLectura = DateTime.Now;
        _uow.AlertaRachaEmocional.Actualizar(alerta);
        await _uow.SaveChangesAsync();

        return Ok(new { mensaje = "Alerta marcada como leída.", alerta = Mapear(alerta) });
    }

    /// <summary>Marcar alerta como resuelta con notas opcionales.</summary>
    [HttpPut("marcar-resuelta/{id:int}")]
    public async Task<IActionResult> MarcarResuelta(int id, [FromBody] ResolverAlertaDto? dto)
    {
        if (!TryObtenerPsicologoId(out var psiId))
            return Unauthorized();

        var alerta = await _uow.AlertaRachaEmocional.ObtenerPrimero(
            a => a.AreCodigo == id && a.ArePsicologoFk == psiId && a.AreEstadoRegistro == "activo");

        if (alerta == null)
            return NotFound("Alerta no encontrada.");

        alerta.AreEstado = "resuelta";
        alerta.AreFechaResolucion = DateTime.Now;
        alerta.AreNotasResolucion = dto?.Notas?.Trim();

        if (alerta.AreFechaLectura == null)
            alerta.AreFechaLectura = DateTime.Now;

        _uow.AlertaRachaEmocional.Actualizar(alerta);
        await _uow.SaveChangesAsync();

        return Ok(new { mensaje = "Alerta marcada como resuelta.", alerta = Mapear(alerta) });
    }

    public class ResolverAlertaDto
    {
        public string? Notas { get; set; }
    }
}
