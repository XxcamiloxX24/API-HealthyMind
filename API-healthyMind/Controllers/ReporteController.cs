using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

namespace API_healthyMind.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "CualquierRol")]
public class ReporteController : ControllerBase
{
    private readonly IUnidadDeTrabajo _uow;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ReporteController> _logger;

    private static readonly string[] EstadosValidos = { "creado", "proceso", "resuelto", "cerrado" };
    private static readonly string[] PrioridadesValidas = { "baja", "media", "alta", "critica" };

    public ReporteController(
        IUnidadDeTrabajo uow,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<ReporteController> logger)
    {
        _uow = uow;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    private string? ObtenerIdUsuarioAutenticado() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("nameid");

    private string? ObtenerRolUsuario() =>
        User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");

    private bool TryObtenerAprendizId(out int id)
    {
        id = 0;
        var userId = ObtenerIdUsuarioAutenticado();
        return !string.IsNullOrWhiteSpace(userId) && int.TryParse(userId, out id);
    }

    private bool TryObtenerPsicologoId(out int id)
    {
        id = 0;
        var userId = ObtenerIdUsuarioAutenticado();
        return !string.IsNullOrWhiteSpace(userId) && int.TryParse(userId, out id);
    }

    private static string FormatearHistorial(ReporteHistorial h)
    {
        var texto = string.IsNullOrWhiteSpace(h.RephDescripcion)
            ? h.RephAccion
            : $"{h.RephAccion}: {h.RephDescripcion}";
        return $"{texto} ({h.RephFecha:dd/MM/yyyy hh:mm tt})";
    }

    private static object MapearAResponse(Reporte r)
    {
        var usuario = "";
        var correo = "";
        if (r.RepTipoReportador == "Aprendiz" && r.RepAprendizFkNavigation != null)
        {
            var a = r.RepAprendizFkNavigation;
            usuario = $"{a.AprNombre} {a.AprApellido}".Trim();
            correo = a.AprCorreoPersonal ?? a.AprCorreoInstitucional ?? "";
        }
        else if (r.RepTipoReportador == "Psicologo" && r.RepPsicologoFkNavigation != null)
        {
            var p = r.RepPsicologoFkNavigation;
            usuario = $"{p.PsiNombre} {p.PsiApellido}".Trim();
            correo = p.PsiCorreoPersonal ?? p.PsiCorreoInstitucional ?? "";
        }

        var historial = (r.ReporteHistorials ?? Enumerable.Empty<ReporteHistorial>())
            .OrderBy(h => h.RephFecha)
            .Select(FormatearHistorial)
            .ToList();

        return new
        {
            id = r.RepCodigo,
            titulo = r.RepTitulo,
            descripcion = r.RepDescripcion,
            fecha = r.RepFechaCreacion.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            estado = r.RepEstado,
            historial,
            usuario,
            correo,
            prioridad = r.RepPrioridad,
            categoria = r.RepCategoria,
            asignadoA = r.RepAsignadoA,
            fechaActualizacion = r.RepFechaActualizacion?.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)
        };
    }

    /// <summary>Devuelve los valores válidos para estado y prioridad (para dropdowns).</summary>
    [HttpGet("estados")]
    [AllowAnonymous]
    public IActionResult ObtenerEstados()
    {
        return Ok(new
        {
            estados = EstadosValidos,
            prioridades = PrioridadesValidas
        });
    }

    /// <summary>Lista reportes. Aprendiz solo ve los suyos; Psicólogo y Admin ven todos.</summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] string? estado, [FromQuery] string? prioridad, [FromQuery] string? categoria)
    {
        IQueryable<Reporte> query = _uow.Reporte.Query()
            .Where(r => r.RepEstadoRegistro == "activo")
            .Include(r => r.RepAprendizFkNavigation)
            .Include(r => r.RepPsicologoFkNavigation)
            .Include(r => r.ReporteHistorials);

        var rol = ObtenerRolUsuario();
        if (rol == Roles.Aprendiz && TryObtenerAprendizId(out var aprId))
        {
            query = query.Where(r => r.RepAprendizFk == aprId);
        }

        if (!string.IsNullOrWhiteSpace(estado))
            query = query.Where(r => r.RepEstado.ToLower() == estado.Trim().ToLower());
        if (!string.IsNullOrWhiteSpace(prioridad))
            query = query.Where(r => r.RepPrioridad.ToLower() == prioridad.Trim().ToLower());
        if (!string.IsNullOrWhiteSpace(categoria))
            query = query.Where(r => r.RepCategoria.ToLower().Contains(categoria.Trim().ToLower()));

        var datos = await query.OrderByDescending(r => r.RepFechaCreacion).ToListAsync();
        var resultados = datos.Select(MapearAResponse);
        return Ok(resultados);
    }

    /// <summary>Obtiene un reporte por ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var reporte = await _uow.Reporte.Query()
            .Include(r => r.RepAprendizFkNavigation)
            .Include(r => r.RepPsicologoFkNavigation)
            .Include(r => r.ReporteHistorials)
            .FirstOrDefaultAsync(r => r.RepCodigo == id && r.RepEstadoRegistro == "activo");

        if (reporte == null)
            return NotFound();

        var rol = ObtenerRolUsuario();
        if (rol == Roles.Aprendiz && TryObtenerAprendizId(out var aprId))
        {
            if (reporte.RepAprendizFk != aprId)
                return Forbid();
        }

        return Ok(MapearAResponse(reporte));
    }

    /// <summary>Crea un reporte. Disponible para Aprendiz y Psicólogo.</summary>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ReporteCreateDTO dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Titulo) || string.IsNullOrWhiteSpace(dto.Descripcion))
            return BadRequest("Se requieren titulo y descripcion.");

        if (string.IsNullOrWhiteSpace(dto.Categoria))
            return BadRequest("La categoria es obligatoria.");

        var prioridad = (dto.Prioridad ?? "media").Trim().ToLower();
        if (!PrioridadesValidas.Contains(prioridad))
            prioridad = "media";

        var rol = ObtenerRolUsuario();
        int? aprFk = null;
        int? psiFk = null;
        string tipoReportador;

        if (rol == Roles.Aprendiz && TryObtenerAprendizId(out var aprId))
        {
            aprFk = aprId;
            tipoReportador = "Aprendiz";
        }
        else if (rol == Roles.Psicologo && TryObtenerPsicologoId(out var psiId))
        {
            psiFk = psiId;
            tipoReportador = "Psicologo";
        }
        else
        {
            return Forbid();
        }

        var ahora = DateTime.Now;
        var reporte = new Reporte
        {
            RepTitulo = dto.Titulo.Trim(),
            RepDescripcion = dto.Descripcion.Trim(),
            RepFechaCreacion = ahora,
            RepEstado = "creado",
            RepPrioridad = prioridad,
            RepCategoria = dto.Categoria.Trim(),
            RepAsignadoA = "Administrador",
            RepFechaActualizacion = ahora,
            RepTipoReportador = tipoReportador,
            RepAprendizFk = aprFk,
            RepPsicologoFk = psiFk,
            RepEstadoRegistro = "activo"
        };

        reporte.ReporteHistorials.Add(new ReporteHistorial
        {
            RephAccion = "Creado",
            RephDescripcion = null,
            RephFecha = ahora
        });

        await _uow.Reporte.Agregar(reporte);
        await _uow.SaveChangesAsync();

        var creado = await _uow.Reporte.Query()
            .Include(r => r.RepAprendizFkNavigation)
            .Include(r => r.RepPsicologoFkNavigation)
            .Include(r => r.ReporteHistorials)
            .FirstOrDefaultAsync(r => r.RepCodigo == reporte.RepCodigo);

        _ = NotificarAdministradoresAsync(creado!);

        return CreatedAtAction(nameof(ObtenerPorId), new { id = reporte.RepCodigo }, MapearAResponse(creado!));
    }

    /// <summary>
    /// Envío de correo a los administradores tras la creación del reporte.
    /// Corre en background y nunca propaga excepciones para no afectar al cliente.
    /// </summary>
    private async Task NotificarAdministradoresAsync(Reporte reporte)
    {
        try
        {
            var destinos = ObtenerCorreosAdmin();
            if (destinos.Count == 0)
            {
                _logger.LogInformation("No hay correos de administrador configurados; se omite notificación del reporte {Id}.", reporte.RepCodigo);
                return;
            }

            var (usuario, correoReportador) = ObtenerDatosReportador(reporte);
            var asunto = $"[Healthy Mind] Nuevo reporte #{reporte.RepCodigo} — {reporte.RepTitulo}";
            var cuerpo = ConstruirCuerpoCorreo(reporte, usuario, correoReportador);

            foreach (var destino in destinos)
            {
                try
                {
                    await _emailService.SendAsync(destino, asunto, cuerpo, isHtml: true);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Fallo enviando correo de reporte {Id} a {Destino}.", reporte.RepCodigo, destino);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Fallo general notificando reporte {Id} a administradores.", reporte.RepCodigo);
        }
    }

    private List<string> ObtenerCorreosAdmin()
    {
        var lista = new List<string>();

        var configurados = _configuration["Reportes:AdminNotifyEmails"];
        if (!string.IsNullOrWhiteSpace(configurados))
        {
            foreach (var item in configurados.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (!string.IsNullOrWhiteSpace(item) && !lista.Contains(item, StringComparer.OrdinalIgnoreCase))
                    lista.Add(item);
            }
        }

        var adminEmail = _configuration["settings:adminEmail"];
        if (!string.IsNullOrWhiteSpace(adminEmail) && !lista.Contains(adminEmail, StringComparer.OrdinalIgnoreCase))
            lista.Add(adminEmail);

        return lista;
    }

    private static (string Usuario, string Correo) ObtenerDatosReportador(Reporte r)
    {
        if (r.RepTipoReportador == "Aprendiz" && r.RepAprendizFkNavigation != null)
        {
            var a = r.RepAprendizFkNavigation;
            var nombre = $"{a.AprNombre} {a.AprApellido}".Trim();
            var correo = a.AprCorreoPersonal ?? a.AprCorreoInstitucional ?? "";
            return (nombre, correo);
        }
        if (r.RepTipoReportador == "Psicologo" && r.RepPsicologoFkNavigation != null)
        {
            var p = r.RepPsicologoFkNavigation;
            var nombre = $"{p.PsiNombre} {p.PsiApellido}".Trim();
            var correo = p.PsiCorreoPersonal ?? p.PsiCorreoInstitucional ?? "";
            return (nombre, correo);
        }
        return ("", "");
    }

    private static string ConstruirCuerpoCorreo(Reporte r, string usuario, string correoReportador)
    {
        var descripcionHtml = System.Net.WebUtility.HtmlEncode(r.RepDescripcion).Replace("\n", "<br>");
        var tituloHtml = System.Net.WebUtility.HtmlEncode(r.RepTitulo);
        var usuarioHtml = System.Net.WebUtility.HtmlEncode(usuario);
        var correoHtml = System.Net.WebUtility.HtmlEncode(correoReportador);
        var categoriaHtml = System.Net.WebUtility.HtmlEncode(r.RepCategoria);

        return $@"
            <div style=""font-family:Segoe UI,Arial,sans-serif;color:#1e293b;max-width:640px;"">
                <h2 style=""margin-bottom:8px;"">Nuevo reporte en Healthy Mind</h2>
                <p style=""margin-top:0;color:#475569;"">Se registró un nuevo reporte que requiere gestión.</p>
                <table style=""border-collapse:collapse;margin:16px 0;width:100%;"">
                    <tr><td style=""padding:6px 8px;color:#64748b;"">ID</td><td style=""padding:6px 8px;""><strong>#{r.RepCodigo}</strong></td></tr>
                    <tr><td style=""padding:6px 8px;color:#64748b;"">Título</td><td style=""padding:6px 8px;"">{tituloHtml}</td></tr>
                    <tr><td style=""padding:6px 8px;color:#64748b;"">Categoría</td><td style=""padding:6px 8px;"">{categoriaHtml}</td></tr>
                    <tr><td style=""padding:6px 8px;color:#64748b;"">Prioridad</td><td style=""padding:6px 8px;"">{r.RepPrioridad}</td></tr>
                    <tr><td style=""padding:6px 8px;color:#64748b;"">Reportador</td><td style=""padding:6px 8px;"">{r.RepTipoReportador} — {usuarioHtml} ({correoHtml})</td></tr>
                    <tr><td style=""padding:6px 8px;color:#64748b;"">Fecha</td><td style=""padding:6px 8px;"">{r.RepFechaCreacion:dd/MM/yyyy HH:mm}</td></tr>
                </table>
                <p style=""margin-bottom:4px;""><strong>Descripción</strong></p>
                <div style=""padding:12px;background:#f8fafc;border-radius:8px;border:1px solid #e2e8f0;"">{descripcionHtml}</div>
                <p style=""margin-top:20px;color:#64748b;font-size:12px;"">Ingresa al panel de administración para gestionar este reporte.</p>
            </div>";
    }

    /// <summary>Cambia el estado del reporte. Solo Administrador o Psicólogo.</summary>
    [HttpPatch("{id:int}/estado")]
    [Authorize(Policy = "AdministradorYPsicologo")]
    public async Task<IActionResult> ActualizarEstado(int id, [FromBody] ReporteUpdateEstadoDTO dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Estado))
            return BadRequest("Se requiere el campo estado.");

        var nuevoEstado = dto.Estado.Trim().ToLower();
        if (!EstadosValidos.Contains(nuevoEstado))
            return BadRequest($"Estado invalido. Validos: {string.Join(", ", EstadosValidos)}");

        var reporte = await _uow.Reporte.ObtenerPorID(id);
        if (reporte == null || reporte.RepEstadoRegistro != "activo")
            return NotFound();

        var estadoAnterior = reporte.RepEstado;
        reporte.RepEstado = nuevoEstado;
        reporte.RepFechaActualizacion = DateTime.Now;

        _uow.Reporte.Actualizar(reporte);

        var ctx = _uow.ObtenerContexto();
        ctx.ReporteHistorials.Add(new ReporteHistorial
        {
            RephReporteFk = id,
            RephAccion = "Estado cambiado a " + nuevoEstado,
            RephDescripcion = null,
            RephFecha = DateTime.Now
        });

        await _uow.SaveChangesAsync();

        var actualizado = await _uow.Reporte.Query()
            .Include(r => r.RepAprendizFkNavigation)
            .Include(r => r.RepPsicologoFkNavigation)
            .Include(r => r.ReporteHistorials)
            .FirstOrDefaultAsync(r => r.RepCodigo == id);

        return Ok(MapearAResponse(actualizado!));
    }

    /// <summary>Cambia la prioridad del reporte. Solo Administrador o Psicólogo.</summary>
    [HttpPatch("{id:int}/prioridad")]
    [Authorize(Policy = "AdministradorYPsicologo")]
    public async Task<IActionResult> ActualizarPrioridad(int id, [FromBody] ReporteUpdatePrioridadDTO dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Prioridad))
            return BadRequest("Se requiere el campo prioridad.");

        var nuevaPrioridad = dto.Prioridad.Trim().ToLower();
        if (!PrioridadesValidas.Contains(nuevaPrioridad))
            return BadRequest($"Prioridad invalida. Validas: {string.Join(", ", PrioridadesValidas)}");

        var reporte = await _uow.Reporte.ObtenerPorID(id);
        if (reporte == null || reporte.RepEstadoRegistro != "activo")
            return NotFound();

        reporte.RepPrioridad = nuevaPrioridad;
        reporte.RepFechaActualizacion = DateTime.Now;

        _uow.Reporte.Actualizar(reporte);

        var ctx = _uow.ObtenerContexto();
        ctx.ReporteHistorials.Add(new ReporteHistorial
        {
            RephReporteFk = id,
            RephAccion = "Prioridad cambiada a " + nuevaPrioridad,
            RephDescripcion = null,
            RephFecha = DateTime.Now
        });

        await _uow.SaveChangesAsync();

        var actualizado = await _uow.Reporte.Query()
            .Include(r => r.RepAprendizFkNavigation)
            .Include(r => r.RepPsicologoFkNavigation)
            .Include(r => r.ReporteHistorials)
            .FirstOrDefaultAsync(r => r.RepCodigo == id);

        return Ok(MapearAResponse(actualizado!));
    }

    /// <summary>Agrega un comentario al reporte. Solo Administrador o Psicólogo.</summary>
    [HttpPost("{id:int}/comentario")]
    [Authorize(Policy = "AdministradorYPsicologo")]
    public async Task<IActionResult> AgregarComentario(int id, [FromBody] ReporteComentarioDTO dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Comentario))
            return BadRequest("Se requiere el campo comentario.");

        var reporte = await _uow.Reporte.ObtenerPorID(id);
        if (reporte == null || reporte.RepEstadoRegistro != "activo")
            return NotFound();

        reporte.RepFechaActualizacion = DateTime.Now;
        _uow.Reporte.Actualizar(reporte);

        var ctx = _uow.ObtenerContexto();
        ctx.ReporteHistorials.Add(new ReporteHistorial
        {
            RephReporteFk = id,
            RephAccion = "Comentario agregado",
            RephDescripcion = dto.Comentario.Trim(),
            RephFecha = DateTime.Now
        });

        await _uow.SaveChangesAsync();

        var actualizado = await _uow.Reporte.Query()
            .Include(r => r.RepAprendizFkNavigation)
            .Include(r => r.RepPsicologoFkNavigation)
            .Include(r => r.ReporteHistorials)
            .FirstOrDefaultAsync(r => r.RepCodigo == id);

        return Ok(MapearAResponse(actualizado!));
    }
}
