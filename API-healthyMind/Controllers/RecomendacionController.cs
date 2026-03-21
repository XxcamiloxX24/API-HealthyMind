using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_healthyMind.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdministradorYPsicologo")]
public class RecomendacionController : ControllerBase
{
    private readonly IUnidadDeTrabajo _uow;

    public static readonly string[] EstadosValidos = { "Pendiente", "En Progreso", "Completada" };

    public RecomendacionController(IUnidadDeTrabajo uow)
    {
        _uow = uow;
    }

    private bool TryObtenerPsicologoId(out int psicologoId)
    {
        psicologoId = 0;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("nameid");
        return !string.IsNullOrWhiteSpace(userId) && int.TryParse(userId, out psicologoId);
    }

    private static string? NormalizarEstado(string? estado)
    {
        if (string.IsNullOrWhiteSpace(estado)) return null;
        var e = estado.Trim();
        if (string.Equals(e, "Pendiente", StringComparison.OrdinalIgnoreCase)) return "Pendiente";
        if (string.Equals(e, "En Progreso", StringComparison.OrdinalIgnoreCase) || string.Equals(e, "En progreso", StringComparison.OrdinalIgnoreCase)) return "En Progreso";
        if (string.Equals(e, "Completada", StringComparison.OrdinalIgnoreCase)) return "Completada";
        return null;
    }

    /// <summary>Devuelve los valores válidos para el campo estado (para dropdowns).</summary>
    [HttpGet("estados")]
    [AllowAnonymous]
    public IActionResult ObtenerEstados()
    {
        return Ok(EstadosValidos.Select(e => new { valor = e }));
    }

    /// <summary>Lista las recomendaciones activas de un seguimiento. El psicólogo solo ve las de sus seguimientos.</summary>
    [HttpGet("por-seguimiento/{seguimientoId:int}")]
    public async Task<IActionResult> ListarPorSeguimiento(int seguimientoId)
    {
        var query = _uow.Recomendacion.Query()
            .Where(r => r.RecSeguimientoFk == seguimientoId && r.RecEstadoRegistro == "activo")
            .OrderByDescending(r => r.RecFechaCreacion);

        if (User.IsInRole(Roles.Psicologo) && TryObtenerPsicologoId(out var psicologoId))
        {
            var seguimiento = await _uow.SeguimientoAprendiz.Query()
                .Where(s => s.SegCodigo == seguimientoId && s.SegEstadoRegistro == "activo")
                .FirstOrDefaultAsync();
            if (seguimiento == null)
                return NotFound("No se encontró el seguimiento.");
            if (seguimiento.SegPsicologoFk != psicologoId)
                return Forbid();
        }

        var datos = await query.ToListAsync();
        var resultados = datos.Select(r => new
        {
            r.RecCodigo,
            r.RecSeguimientoFk,
            r.RecTitulo,
            r.RecDescripcion,
            r.RecFechaVencimiento,
            r.RecEstado,
            r.RecFechaCreacion,
            r.RecFechaActualizacion
        });

        return Ok(resultados);
    }

    /// <summary>Obtiene una recomendación por ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var rec = await _uow.Recomendacion.Query()
            .Where(r => r.RecCodigo == id && r.RecEstadoRegistro == "activo")
            .FirstOrDefaultAsync();

        if (rec == null)
            return NotFound("No se encontró la recomendación.");

        if (User.IsInRole(Roles.Psicologo) && TryObtenerPsicologoId(out var psicologoId))
        {
            var seguimiento = await _uow.SeguimientoAprendiz.Query()
                .Where(s => s.SegCodigo == rec.RecSeguimientoFk)
                .FirstOrDefaultAsync();
            if (seguimiento?.SegPsicologoFk != psicologoId)
                return Forbid();
        }

        return Ok(new
        {
            rec.RecCodigo,
            rec.RecSeguimientoFk,
            rec.RecTitulo,
            rec.RecDescripcion,
            rec.RecFechaVencimiento,
            rec.RecEstado,
            rec.RecFechaCreacion,
            rec.RecFechaActualizacion
        });
    }

    /// <summary>Crea una nueva recomendación para un seguimiento.</summary>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] RecomendacionDTO dto)
    {
        if (dto == null)
            return BadRequest("El cuerpo no debe estar vacío.");

        if (dto.RecSeguimientoFk == null || dto.RecSeguimientoFk <= 0)
            return BadRequest("RecSeguimientoFk es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.RecTitulo))
            return BadRequest("RecTitulo es obligatorio.");

        var estado = NormalizarEstado(dto.RecEstado);
        if (dto.RecEstado != null && estado == null)
            return BadRequest($"RecEstado debe ser uno de: {string.Join(", ", EstadosValidos)}.");

        var seguimiento = await _uow.SeguimientoAprendiz.Query()
            .Where(s => s.SegCodigo == dto.RecSeguimientoFk && s.SegEstadoRegistro == "activo")
            .FirstOrDefaultAsync();

        if (seguimiento == null)
            return NotFound("No se encontró el seguimiento.");

        if (User.IsInRole(Roles.Psicologo) && TryObtenerPsicologoId(out var psicologoId))
        {
            if (seguimiento.SegPsicologoFk != psicologoId)
                return Forbid();
        }

        var ahora = DateTime.UtcNow;
        var nueva = new Recomendacion
        {
            RecSeguimientoFk = dto.RecSeguimientoFk.Value,
            RecTitulo = dto.RecTitulo.Trim(),
            RecDescripcion = string.IsNullOrWhiteSpace(dto.RecDescripcion) ? null : dto.RecDescripcion.Trim(),
            RecFechaVencimiento = null,
            RecEstado = estado ?? "Pendiente",
            RecEstadoRegistro = "activo",
            RecFechaCreacion = ahora,
            RecFechaActualizacion = ahora
        };

        await _uow.Recomendacion.Agregar(nueva);
        await _uow.SaveChangesAsync();

        return Ok(new
        {
            mensaje = "Recomendación creada exitosamente",
            recCodigo = nueva.RecCodigo,
            recTitulo = nueva.RecTitulo,
            recEstado = nueva.RecEstado
        });
    }

    /// <summary>Actualiza una recomendación existente.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] RecomendacionDTO dto)
    {
        if (dto == null)
            return BadRequest("El cuerpo no debe estar vacío.");

        var rec = await _uow.Recomendacion.Query()
            .Where(r => r.RecCodigo == id && r.RecEstadoRegistro == "activo")
            .FirstOrDefaultAsync();

        if (rec == null)
            return NotFound("No se encontró la recomendación.");

        if (User.IsInRole(Roles.Psicologo) && TryObtenerPsicologoId(out var psicologoId))
        {
            var seguimiento = await _uow.SeguimientoAprendiz.Query()
                .Where(s => s.SegCodigo == rec.RecSeguimientoFk)
                .FirstOrDefaultAsync();
            if (seguimiento?.SegPsicologoFk != psicologoId)
                return Forbid();
        }

        if (!string.IsNullOrWhiteSpace(dto.RecTitulo))
            rec.RecTitulo = dto.RecTitulo.Trim();

        rec.RecDescripcion = string.IsNullOrWhiteSpace(dto.RecDescripcion) ? null : dto.RecDescripcion.Trim();

        if (dto.RecEstado != null)
        {
            var estado = NormalizarEstado(dto.RecEstado);
            if (estado == null)
                return BadRequest($"RecEstado debe ser uno de: {string.Join(", ", EstadosValidos)}.");
            var estadoAnterior = rec.RecEstado;
            rec.RecEstado = estado;
            if (estado == "Completada" && estadoAnterior != "Completada")
                rec.RecFechaVencimiento = DateTime.UtcNow;
        }

        rec.RecFechaActualizacion = DateTime.UtcNow;
        _uow.Recomendacion.Actualizar(rec);
        await _uow.SaveChangesAsync();

        return Ok(new
        {
            mensaje = "Recomendación actualizada correctamente",
            recCodigo = rec.RecCodigo
        });
    }

    /// <summary>Elimina (soft delete) una recomendación.</summary>
    [HttpPut("eliminar/{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var rec = await _uow.Recomendacion.Query()
            .Where(r => r.RecCodigo == id && r.RecEstadoRegistro == "activo")
            .FirstOrDefaultAsync();

        if (rec == null)
            return NotFound("No se encontró la recomendación.");

        if (User.IsInRole(Roles.Psicologo) && TryObtenerPsicologoId(out var psicologoId))
        {
            var seguimiento = await _uow.SeguimientoAprendiz.Query()
                .Where(s => s.SegCodigo == rec.RecSeguimientoFk)
                .FirstOrDefaultAsync();
            if (seguimiento?.SegPsicologoFk != psicologoId)
                return Forbid();
        }

        rec.RecEstadoRegistro = "inactivo";
        _uow.Recomendacion.Actualizar(rec);
        await _uow.SaveChangesAsync();

        return Ok("Se ha eliminado correctamente.");
    }
}
