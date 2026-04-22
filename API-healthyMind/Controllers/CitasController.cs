using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Models.DTO.Filtros;
using API_healthyMind.Repositorios.Interfaces;
using API_healthyMind.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using System.Text;

using static API_healthyMind.Controllers.AprendizController;
namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CualquierRol")]
    public class CitasController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;
        private readonly IEmailService _emailService;
        private readonly IChatPushService _chatPush;
        private readonly IConfiguration _configuration;


        public CitasController(
            IUnidadDeTrabajo uow,
            IEmailService emailService,
            IChatPushService chatPush,
            IConfiguration configuration)
        {
            _uow = uow;
            _emailService = emailService;
            _chatPush = chatPush;
            _configuration = configuration;
        }

        private const string EstadoPendiente = "pendiente";
        private const string EstadoProgramada = "programada";
        private const string EstadoReprogramada = "reprogramada";
        private const string EstadoCancelada = "cancelada";
        private const string EstadoRealizada = "realizada";
        private const string EstadoNoAsistio = "no asistió";

        private static readonly string[] EstadosSolicitudActiva =
        {
            EstadoPendiente,
            EstadoProgramada,
            EstadoReprogramada
        };

        private static readonly string[] TiposCitaPermitidos =
        {
            "videollamada",
            "chat",
            "presencial"
        };

        private string? ObtenerIdUsuarioAutenticado()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("nameid");
        }

        private bool TryObtenerAprendizIdAutenticado(out int aprendizId)
        {
            aprendizId = 0;
            var userId = ObtenerIdUsuarioAutenticado();

            return !string.IsNullOrWhiteSpace(userId) && int.TryParse(userId, out aprendizId);
        }

        private bool TryObtenerPsicologoIdAutenticado(out int psicologoId)
        {
            psicologoId = 0;
            var userId = ObtenerIdUsuarioAutenticado();
            return !string.IsNullOrWhiteSpace(userId) && int.TryParse(userId, out psicologoId);
        }

        private async Task<AprendizFicha?> ObtenerAprendizFichaActivaAsync(int aprendizId)
        {
            return await _uow.AprendizFicha.Query()
                .Include(c => c.Aprendiz)
                    .ThenInclude(c => c.EstadoAprendiz)
                .Include(c => c.Aprendiz)
                    .ThenInclude(c => c.Municipio)
                        .ThenInclude(c => c.Regional)
                .Include(x => x.Ficha)
                    .ThenInclude(f => f.programaFormacion)
                        .ThenInclude(p => p.Area)
                            .ThenInclude(a => a.AreaPsicologo)
                .Include(x => x.Ficha)
                    .ThenInclude(f => f.programaFormacion)
                        .ThenInclude(p => p.Centro)
                .Where(x => x.Aprendiz != null &&
                            x.Aprendiz.AprCodigo == aprendizId &&
                            x.Aprendiz.AprEstadoRegistro == "activo" &&
                            x.AprFicEstadoRegistro == "activo")
                .FirstOrDefaultAsync();
        }

        private static bool EsTipoCitaValido(string? tipoCita)
        {
            return !string.IsNullOrWhiteSpace(tipoCita) &&
                   TiposCitaPermitidos.Contains(tipoCita.Trim().ToLower());
        }

        private static bool EsHorarioValido(TimeOnly? horaInicio, TimeOnly? horaFin)
        {
            if (!horaInicio.HasValue || !horaFin.HasValue)
            {
                return false;
            }

            return horaInicio.Value < horaFin.Value;
        }

        private static string? ValidarDatosAgenda(CitaDTO dto)
        {
            if (!EsTipoCitaValido(dto.CitTipoCita))
            {
                return "El tipo de cita no es válido.";
            }

            if (string.IsNullOrWhiteSpace(dto.CitEstadoCita))
            {
                return "El estado de la cita es obligatorio.";
            }

            var estado = dto.CitEstadoCita.Trim().ToLower();

            if (estado != EstadoPendiente)
            {
                if (!dto.CitFechaProgramada.HasValue)
                {
                    return "La fecha programada es obligatoria para ese estado.";
                }

                if (!EsHorarioValido(dto.CitHoraInicio, dto.CitHoraFin))
                {
                    return "La hora de inicio debe ser menor que la hora de fin.";
                }
            }

            return null;
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

        private static object MapearAprendizFicha(AprendizFicha d)
        {
            return new
            {
                d.AprFicCodigo,
                Aprendiz = MapearAprendiz(d.Aprendiz),
                Ficha = new
                {
                    d.Ficha.FicCodigo,
                    d.Ficha.FicJornada,
                    d.Ficha.FicFechaInicio,
                    d.Ficha.FicFechaFin,
                    d.Ficha.FicEstadoFormacion,
                    ProgramaFormacion = d.Ficha == null ? null : new
                    {
                        d.Ficha.programaFormacion.ProgCodigo,
                        d.Ficha.programaFormacion.ProgNombre,
                        d.Ficha.programaFormacion.ProgModalidad,
                        d.Ficha.programaFormacion.ProgFormaModalidad,
                        d.Ficha.programaFormacion.NivelFormacion,
                        Area = d.Ficha.programaFormacion.Area == null ? null : new
                        {
                            d.Ficha.programaFormacion.Area.AreaCodigo,
                            d.Ficha.programaFormacion.Area.AreaNombre,
                        },
                        d.Ficha.programaFormacion.Centro
                    }
                }

            };
        }

        private static object MapearCita(Cita c)
        {
            return new
            {
                c.CitCodigo,
                c.CitTipoCita,
                c.CitFechaProgramada,
                c.CitHoraInicio,
                c.CitHoraFin,
                CitMotivo = c.CitMotivo ?? c.CitMotivoSolicitud,
                c.CitAnotaciones,
                c.CitEstadoCita,
                c.CitFechaCreacion,
                aprendizCita = MapearAprendizFicha(c.CitAprCodFkNavigation),
                psicologo = c.CitPsiCodFkNavigation,
            };
        }


        // GET: NivelFormacionController
        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.Citas.ObtenerTodoConCondicion(e => e.CitEstadoRegistro == "activo",
                e => e.Include(c => c.CitAprCodFkNavigation)
                        .ThenInclude(c => c.Aprendiz)
                            .ThenInclude(c => c.Municipio)
                                .ThenInclude(c => c.Regional)
                      .Include(c => c.CitAprCodFkNavigation.Aprendiz.EstadoAprendiz)
                      .Include(c => c.CitAprCodFkNavigation.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.CitAprCodFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.CitAprCodFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.CitPsiCodFkNavigation)
                        );
                            
                       

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            var resultados = datos.Select(c => MapearCita(c));

            return Ok(resultados);
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet("listar-todas")]
        public async Task<IActionResult> ListarSeguimientos([FromQuery] PaginacionDTO p)
        {
            if (p.TamanoPagina > 100)
                p.TamanoPagina = 100;

            var query = _uow.Citas.Query()
                        .Include(c => c.CitAprCodFkNavigation)
                        .ThenInclude(c => c.Aprendiz)
                            .ThenInclude(c => c.Municipio)
                                .ThenInclude(c => c.Regional)
                        .Include(c => c.CitAprCodFkNavigation.Aprendiz.EstadoAprendiz)
                        .Include(c => c.CitAprCodFkNavigation.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.CitAprCodFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.CitAprCodFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.CitPsiCodFkNavigation);

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

            var resultados = datos.Select(c => MapearCita(c));

            return Ok(new
            {
                paginaActual = p.Pagina,
                paginaAnterior = p.Pagina > 1 ? p.Pagina - 1 : (int?)null,
                paginaSiguiente = p.Pagina < totalPaginas ? p.Pagina + 1 : (int?)null,
                tamanoPagina = p.TamanoPagina,
                totalRegistros,
                totalPaginas,
                resultados
            });
        }


        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet("listar-activas")]
        public async Task<IActionResult> ListarCitasActivas([FromQuery] PaginacionDTO p)
        {
            if (p.TamanoPagina > 100) // límite de seguridad opcional
                p.TamanoPagina = 100;

            var query = _uow.Citas.Query()
                        .Include(c => c.CitAprCodFkNavigation)
                        .ThenInclude(c => c.Aprendiz)
                            .ThenInclude(c => c.Municipio)
                                .ThenInclude(c => c.Regional)
                        .Include(c => c.CitAprCodFkNavigation.Aprendiz.EstadoAprendiz)
                        .Include(c => c.CitAprCodFkNavigation.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.CitAprCodFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.CitAprCodFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.CitPsiCodFkNavigation)
                        .Where(c => c.CitEstadoRegistro == "activo");

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

            var resultados = datos.Select(c => MapearCita(c));

            return Ok(new
            {
                paginaActual = p.Pagina,
                paginaAnterior = p.Pagina > 1 ? p.Pagina - 1 : (int?)null,
                paginaSiguiente = p.Pagina < totalPaginas ? p.Pagina + 1 : (int?)null,
                tamanoPagina = p.TamanoPagina,
                totalRegistros,
                totalPaginas,
                resultados
            });
        }


        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] FiltroCitasDTO f)
        {
            IQueryable<Cita> q = _uow.Citas.Query()
                .Include(c => c.CitAprCodFkNavigation)
                        .ThenInclude(c => c.Aprendiz)
                            .ThenInclude(c => c.Municipio)
                                .ThenInclude(c => c.Regional)
                        .Include(c => c.CitAprCodFkNavigation.Aprendiz.EstadoAprendiz)
                        .Include(c => c.CitAprCodFkNavigation.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.CitAprCodFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.CitAprCodFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.CitPsiCodFkNavigation);

            // ======================
            //    FILTROS
            // ======================

            if (f.FichaCodigo.HasValue)
                q = q.Where(x => x.CitAprCodFkNavigation.Ficha.FicCodigo == f.FichaCodigo.Value);

            if (!string.IsNullOrEmpty(f.AreaNombre))
                q = q.Where(x => x.CitAprCodFkNavigation.Ficha.programaFormacion.Area.AreaNombre.ToLower()
                    .Contains(f.AreaNombre.ToLower()));

            if (!string.IsNullOrEmpty(f.ProgramaNombre))
                q = q.Where(x => x.CitAprCodFkNavigation.Ficha.programaFormacion.ProgNombre.ToLower()
                    .Contains(f.ProgramaNombre.ToLower()));

            if (!string.IsNullOrEmpty(f.PsicologoDocumento))
                q = q.Where(x => x.CitPsiCodFkNavigation.PsiDocumento == f.PsicologoDocumento);

            if (!string.IsNullOrEmpty(f.TipoPoblacion))
                q = q.Where(x => x.CitAprCodFkNavigation.Aprendiz.AprTipoPoblacion.ToLower() == f.TipoPoblacion.ToLower());

            if (f.EstadoAprendizID.HasValue)
                q = q.Where(x => x.CitAprCodFkNavigation.Aprendiz.EstadoAprendiz.EstAprCodigo == f.EstadoAprendizID.Value);

            if (!string.IsNullOrEmpty(f.CentroNombre))
                q = q.Where(x => x.CitAprCodFkNavigation.Ficha.programaFormacion.Centro.CenNombre.ToLower()
                    .Contains(f.CentroNombre.ToLower()));

            if (!string.IsNullOrEmpty(f.Jornada))
                q = q.Where(x => x.CitAprCodFkNavigation.Ficha.FicJornada.ToLower() == f.Jornada.ToLower());


            if (f.fechaProgramada.HasValue)
            {
                var porDia = f.fechaProgramada.Value.Day;
                q = q.Where(x => x.CitFechaProgramada.HasValue && x.CitFechaProgramada.Value.Day == porDia);
            }

            if (f.fechaProgramada.HasValue)
            {
                var PorMes = f.fechaProgramada.Value.Month;
                q = q.Where(x => x.CitFechaProgramada.HasValue && x.CitFechaProgramada.Value.Month == PorMes);
            }

            if (!string.IsNullOrEmpty(f.TipoCita))
                q = q.Where(x => x.CitTipoCita.ToLower() == f.TipoCita.ToLower());

            if (!string.IsNullOrEmpty(f.EstadoCita))
                q = q.Where(x => x.CitEstadoCita.ToLower() == f.EstadoCita.ToLower());


            q = q.Where(x => x.CitEstadoRegistro.ToLower() == "activo");


            var datos = await q.ToListAsync();

            if (!datos.Any())
                return NotFound("No se encontraron resultados con esos filtros.");

            var resultado = datos.Select(c => MapearCita(c)); 

            return Ok(resultado);
        }



        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet("estadistica/comparacion-semanal")]
        public async Task<IActionResult> GetComparacionSemanal(
            [FromQuery] int psicologoCodigo,
            [FromQuery] string? estadoCita,
            [FromQuery] DateOnly? fechaReferencia)
        {
            if (psicologoCodigo <= 0)
            {
                return BadRequest("psicologoCodigo debe ser mayor a 0.");
            }

            var fechaBase = fechaReferencia ?? DateOnly.FromDateTime(DateTime.Today);
            var offset = ((int)fechaBase.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            var inicioSemanaActual = fechaBase.AddDays(-offset);
            var finSemanaActual = inicioSemanaActual.AddDays(6);
            var inicioSemanaAnterior = inicioSemanaActual.AddDays(-7);
            var finSemanaAnterior = inicioSemanaActual.AddDays(-1);

            var estadoFiltro = string.IsNullOrWhiteSpace(estadoCita) ? null : estadoCita.Trim().ToLower();

            var query = _uow.Citas.Query()
                .Where(c => c.CitEstadoRegistro == "activo" &&
                            c.CitPsiCodFk == psicologoCodigo &&
                            c.CitFechaProgramada != null &&
                            c.CitFechaProgramada >= inicioSemanaAnterior &&
                            c.CitFechaProgramada <= finSemanaActual);

            if (estadoFiltro != null)
            {
                query = query.Where(c => c.CitEstadoCita != null && c.CitEstadoCita.ToLower() == estadoFiltro);
            }

            var citas = await query
                .Select(c => new { c.CitFechaProgramada })
                .ToListAsync();

            var conteoSemanaActual = new int[7];
            var conteoSemanaAnterior = new int[7];

            foreach (var cita in citas)
            {
                var fecha = cita.CitFechaProgramada!.Value;
                var indiceDia = ((int)fecha.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;

                if (fecha >= inicioSemanaActual && fecha <= finSemanaActual)
                {
                    conteoSemanaActual[indiceDia]++;
                }
                else if (fecha >= inicioSemanaAnterior && fecha <= finSemanaAnterior)
                {
                    conteoSemanaAnterior[indiceDia]++;
                }
            }

            var dias = new[] { "Lun", "Mar", "Mie", "Jue", "Vie", "Sab", "Dom" };

            var semanaActual = dias.Select((d, i) => new
            {
                dia = d,
                cantidad = conteoSemanaActual[i]
            });

            var semanaAnterior = dias.Select((d, i) => new
            {
                dia = d,
                cantidad = conteoSemanaAnterior[i]
            });

            return Ok(new
            {
                psicologoCodigo,
                estadoCita = string.IsNullOrWhiteSpace(estadoCita) ? "todas" : estadoCita.Trim(),
                rangoSemanaActual = new { inicio = inicioSemanaActual, fin = finSemanaActual },
                rangoSemanaAnterior = new { inicio = inicioSemanaAnterior, fin = finSemanaAnterior },
                semanaActual = new { total = conteoSemanaActual.Sum(), dias = semanaActual },
                semanaAnterior = new { total = conteoSemanaAnterior.Sum(), dias = semanaAnterior }
            });
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet("estadistica/por-mes")]
        public async Task<IActionResult> GetSeguimientosIniciadosPorMes()
        {
            var resultado = await _uow.Citas.Query()
                .Where(x => x.CitEstadoRegistro == "activo" &&
                    x.CitEstadoCita != EstadoPendiente &&
                    x.CitFechaProgramada != null)
                .GroupBy(x => new
                {
                    Año = x.CitFechaProgramada.Value.Year,
                    Mes = x.CitFechaProgramada.Value.Month
                })
                .Select(g => new
                {
                    Año = g.Key.Año,
                    Mes = g.Key.Mes,
                    Total_Citas = g.Count()
                })
                .OrderBy(x => x.Año)
                .ThenBy(x => x.Mes)
                .ToListAsync();

            return Ok(resultado);
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet("estadistica/actividad-exitosa")]
        public async Task<IActionResult> GetActividadExitosa()
        {
            var total = await _uow.Citas.Query().CountAsync(c => c.CitEstadoRegistro == "activo");
            var exitosas = await _uow.Citas.Query().CountAsync(c => c.CitEstadoRegistro == "activo" && c.CitEstadoCita == EstadoRealizada);

            double porcentaje = total == 0 ? 0 : (double)exitosas / total * 100;

            return Ok(new
            {
                totalCitas = total,
                exitosas,
                porcentaje = Math.Round(porcentaje, 2)
            });
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet("citas/estado-proceso")]
        public async Task<IActionResult> GetCitasEnProceso()
        {
            // Estados considerados "en proceso"
            var estadosProceso = new[] { EstadoPendiente, EstadoProgramada, EstadoReprogramada };

            // Total de citas
            var totalCitas = await _uow.Citas.Query().CountAsync(c => c.CitEstadoRegistro == "activo");

            // Si no hay citas, evitar división entre cero
            if (totalCitas == 0)
            {
                return Ok(new
                {
                    totalCitas = 0,
                    citasEnProceso = 0,
                    porcentajeEnProceso = 0,
                    detalle = new object[] { }
                });
            }

            // Citas en proceso
            var detalle = await _uow.Citas.Query()
                .Where(c => c.CitEstadoRegistro == "activo" && estadosProceso.Contains(c.CitEstadoCita))
                .GroupBy(c => c.CitEstadoCita)
                .Select(g => new
                {
                    estado = g.Key,
                    cantidad = g.Count()
                })
                .ToListAsync();

            var citasEnProceso = detalle.Sum(d => d.cantidad);

            // Calcular porcentaje
            var porcentaje = Math.Round(((double)citasEnProceso / totalCitas) * 100, 2);

            return Ok(new
            {
                totalCitas,
                citasEnProceso,
                porcentajeEnProceso = porcentaje,
                detalle
            });
        }


        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet("citas/estado-incidencias")]
        public async Task<IActionResult> GetCitasincidencias()
        {
            // Estados considerados "en proceso"
            var estadosProceso = new[] { EstadoCancelada, EstadoNoAsistio };

            // Total de citas
            var totalCitas = await _uow.Citas.Query().CountAsync(c => c.CitEstadoRegistro == "activo");

            // Si no hay citas, evitar división entre cero
            if (totalCitas == 0)
            {
                return Ok(new
                {
                    totalCitas = 0,
                    citasEnProceso = 0,
                    porcentajeEnProceso = 0,
                    detalle = new object[] { }
                });
            }

            // Citas en proceso
            var detalle = await _uow.Citas.Query()
                .Where(c => c.CitEstadoRegistro == "activo" && estadosProceso.Contains(c.CitEstadoCita))
                .GroupBy(c => c.CitEstadoCita)
                .Select(g => new
                {
                    estado = g.Key,
                    cantidad = g.Count()
                })
                .ToListAsync();

            var citasEnIncidencias = detalle.Sum(d => d.cantidad);

            // Calcular porcentaje
            var porcentaje = Math.Round(((double)citasEnIncidencias / totalCitas) * 100, 2);

            return Ok(new
            {
                totalCitas,
                citasEnIncidencias,
                porcentajeEnProceso = porcentaje,
                detalle
            });
        }


        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet("estadistica/por-estado")]
        public async Task<IActionResult> GetCantidadCitasPorEstado()
        {
            var resultado = await _uow.Citas.Query()
                .Where(x => x.CitEstadoRegistro == "activo")
                .GroupBy(x => x.CitEstadoCita)
                .Select(g => new {
                    EstadoCita = g.Key,      // Programada, Reprogramada, etc.
                    Total = g.Count()
                })
                .OrderBy(x => x.EstadoCita)
                .ToListAsync();

            return Ok(resultado);
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet("estadistica/por-dia")]
        public async Task<IActionResult> GetSeguimientosFinalizadosPorMes()
        {
            var resultado = await _uow.Citas.Query()
                .Where(x => x.CitEstadoRegistro == "activo" &&
                    x.CitEstadoCita != EstadoPendiente &&
                    x.CitFechaProgramada != null)
                .GroupBy(x => new
                {
                    Año = x.CitFechaProgramada.Value.Year,
                    Mes = x.CitFechaProgramada.Value.Month,
                    dia = x.CitFechaProgramada.Value.Day
                })
                .Select(g => new
                {
                    Año = g.Key.Año,
                    Mes = g.Key.Mes,
                    dia = g.Key.dia,
                    Total_Citas = g.Count()
                })
                .OrderBy(x => x.Año)
                .ThenBy(x => x.dia)
                .ToListAsync();

            return Ok(resultado);
        }

        /// <summary>
        /// Citas del psicólogo autenticado en un rango de fechas. Ideal para el calendario/agenda.
        /// GET api/Citas/agenda?desde=2026-03-16&amp;hasta=2026-03-22
        /// </summary>
        [Authorize(Roles = Roles.Psicologo)]
        [HttpGet("agenda")]
        public async Task<IActionResult> ObtenerAgendaPsicologo(
            [FromQuery] DateOnly? desde,
            [FromQuery] DateOnly? hasta)
        {
            if (!TryObtenerPsicologoIdAutenticado(out var psicologoId))
            {
                return Forbid();
            }

            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var inicio = desde ?? hoy.AddDays(-7);
            var fin = hasta ?? hoy.AddDays(30);
            if (fin < inicio)
            {
                (inicio, fin) = (fin, inicio);
            }
            var diasMax = 90;
            if ((fin.ToDateTime(TimeOnly.MinValue) - inicio.ToDateTime(TimeOnly.MinValue)).TotalDays > diasMax)
            {
                fin = inicio.AddDays(diasMax);
            }

            var datos = await _uow.Citas.Query()
                .Include(c => c.CitAprCodFkNavigation)
                    .ThenInclude(c => c.Aprendiz)
                        .ThenInclude(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                .Include(c => c.CitAprCodFkNavigation.Aprendiz.EstadoAprendiz)
                .Include(c => c.CitAprCodFkNavigation.Ficha)
                    .ThenInclude(c => c.programaFormacion)
                        .ThenInclude(c => c.Centro)
                .Include(c => c.CitAprCodFkNavigation.Ficha)
                    .ThenInclude(c => c.programaFormacion)
                        .ThenInclude(c => c.NivelFormacion)
                .Include(c => c.CitAprCodFkNavigation.Ficha)
                    .ThenInclude(c => c.programaFormacion)
                        .ThenInclude(c => c.Area)
                .Include(c => c.CitPsiCodFkNavigation)
                .Where(c => c.CitEstadoRegistro == "activo" &&
                            c.CitPsiCodFk == psicologoId &&
                            c.CitEstadoCita != null &&
                            c.CitEstadoCita.ToLower() != EstadoPendiente &&
                            c.CitFechaProgramada.HasValue &&
                            c.CitFechaProgramada.Value >= inicio &&
                            c.CitFechaProgramada.Value <= fin)
                .OrderBy(c => c.CitFechaProgramada)
                .ThenBy(c => c.CitHoraInicio)
                .ToListAsync();

            var resultado = datos.Select(c => MapearCita(c));
            return Ok(resultado);
        }

        /// <summary>
        /// Solicitudes de cita pendientes del psicólogo (estudiantes que solicitaron y aún no se han programado).
        /// GET api/Citas/solicitudes-pendientes
        /// </summary>
        [Authorize(Roles = Roles.Psicologo)]
        [HttpGet("solicitudes-pendientes")]
        public async Task<IActionResult> ObtenerSolicitudesPendientes()
        {
            if (!TryObtenerPsicologoIdAutenticado(out var psicologoId))
            {
                return Forbid();
            }

            var datos = await _uow.Citas.Query()
                .Include(c => c.CitAprCodFkNavigation)
                    .ThenInclude(c => c.Aprendiz)
                        .ThenInclude(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                .Include(c => c.CitAprCodFkNavigation.Aprendiz.EstadoAprendiz)
                .Include(c => c.CitAprCodFkNavigation.Ficha)
                    .ThenInclude(c => c.programaFormacion)
                        .ThenInclude(c => c.Centro)
                .Include(c => c.CitAprCodFkNavigation.Ficha)
                    .ThenInclude(c => c.programaFormacion)
                        .ThenInclude(c => c.NivelFormacion)
                .Include(c => c.CitAprCodFkNavigation.Ficha)
                    .ThenInclude(c => c.programaFormacion)
                        .ThenInclude(c => c.Area)
                .Include(c => c.CitPsiCodFkNavigation)
                .Where(c => c.CitEstadoRegistro == "activo" &&
                            c.CitPsiCodFk == psicologoId &&
                            c.CitEstadoCita != null &&
                            c.CitEstadoCita.ToLower() == EstadoPendiente)
                .OrderByDescending(c => c.CitFechaCreacion)
                .ToListAsync();

            var resultado = datos.Select(c => MapearCita(c));
            return Ok(resultado);
        }

        /// <summary>
        /// Citas del psicólogo para iniciar conversación: sólo modalidades no presenciales (chat/videollamada)
        /// y excluyendo las ya realizadas o completadas, para que el psicólogo no abra un chat sobre una sesión cerrada.
        /// GET api/Citas/citas-para-nuevo-chat
        /// </summary>
        [Authorize(Roles = Roles.Psicologo)]
        [HttpGet("citas-para-nuevo-chat")]
        public async Task<IActionResult> ObtenerCitasParaNuevoChat()
        {
            if (!TryObtenerPsicologoIdAutenticado(out var psicologoId))
            {
                return Forbid();
            }

            var estadosExcluidos = new[] { EstadoRealizada, "completada" };
            var tiposNoPresenciales = new[] { "chat", "videollamada" };

            var datos = await _uow.Citas.Query()
                .Include(c => c.CitAprCodFkNavigation)
                    .ThenInclude(c => c.Aprendiz)
                        .ThenInclude(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                .Include(c => c.CitAprCodFkNavigation.Aprendiz.EstadoAprendiz)
                .Include(c => c.CitAprCodFkNavigation.Ficha)
                    .ThenInclude(c => c.programaFormacion)
                        .ThenInclude(c => c.Centro)
                .Include(c => c.CitAprCodFkNavigation.Ficha)
                    .ThenInclude(c => c.programaFormacion)
                        .ThenInclude(c => c.NivelFormacion)
                .Include(c => c.CitAprCodFkNavigation.Ficha)
                    .ThenInclude(c => c.programaFormacion)
                        .ThenInclude(c => c.Area)
                .Include(c => c.CitPsiCodFkNavigation)
                .Where(c => c.CitEstadoRegistro == "activo" &&
                            c.CitPsiCodFk == psicologoId &&
                            c.CitEstadoCita != null &&
                            !estadosExcluidos.Contains(c.CitEstadoCita.Trim().ToLower()) &&
                            c.CitTipoCita != null &&
                            tiposNoPresenciales.Contains(c.CitTipoCita.Trim().ToLower()))
                .OrderByDescending(c => c.CitFechaCreacion)
                .ToListAsync();

            var resultado = datos.Select(c => MapearCita(c));
            return Ok(resultado);
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPost]
        public async Task<IActionResult> CrearRegistro(CitaDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("El cuerpo no debe estar vacio!");
            }

            var errorValidacion = ValidarDatosAgenda(dto);
            if (errorValidacion != null)
            {
                return BadRequest(errorValidacion);
            }

            if (!dto.CitAprCodFk.HasValue || !dto.CitPsiCodFk.HasValue)
            {
                return BadRequest("El aprendiz y el psicólogo son obligatorios.");
            }

            var nuevoReg = new Cita
            {
                CitTipoCita = dto.CitTipoCita!.Trim().ToLower(),
                CitFechaProgramada = dto.CitFechaProgramada,
                CitHoraInicio = dto.CitHoraInicio,
                CitHoraFin = dto.CitHoraFin,
                CitMotivo = dto.CitMotivo,
                CitAnotaciones = dto.CitAnotaciones,
                CitFechaCreacion = DateTime.UtcNow,
                CitEstadoCita = dto.CitEstadoCita!.Trim().ToLower(),
                CitAprCodFk = dto.CitAprCodFk,
                CitPsiCodFk = dto.CitPsiCodFk
            };

            await _uow.Citas.Agregar(nuevoReg);
            await _uow.SaveChangesAsync();

            var datos = (await _uow.Citas.ObtenerTodoConCondicion(c => c.CitCodigo == nuevoReg.CitCodigo,
                c => c.Include(c => c.CitAprCodFkNavigation)
                        .ThenInclude(c => c.Aprendiz)
                            .ThenInclude(c => c.Municipio)
                                .ThenInclude(c => c.Regional)
                        .Include(c => c.CitAprCodFkNavigation.Aprendiz.EstadoAprendiz)
                        .Include(c => c.CitAprCodFkNavigation.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.CitAprCodFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.CitAprCodFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.CitPsiCodFkNavigation))).FirstOrDefault();
            var resultado = MapearCita(datos);
            return Ok(new
            {
                mensaje = "Se ha creado el registro exitosamente",
                resultado
            });
        }

        [Authorize(Policy = "SoloAdministrador")]
        [HttpGet("test-email")]
        public async Task<IActionResult> TestEmail()
        {
            try
            {
                await _emailService.SendAsync("camilovillalobos252@gmail.com", "Test", "Probando...");
                return Ok(new { message = "OK enviado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.ToString() });
            }
        }

        [Authorize(Roles = Roles.Aprendiz)]
        [HttpGet("mis-citas")]
        public async Task<IActionResult> ObtenerMisCitas()
        {
            if (!TryObtenerAprendizIdAutenticado(out var aprendizId))
            {
                return Forbid();
            }

            var datos = await _uow.Citas.Query()
                .Include(c => c.CitAprCodFkNavigation)
                    .ThenInclude(c => c.Aprendiz)
                        .ThenInclude(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                .Include(c => c.CitAprCodFkNavigation.Aprendiz.EstadoAprendiz)
                .Include(c => c.CitAprCodFkNavigation.Ficha)
                    .ThenInclude(c => c.programaFormacion)
                        .ThenInclude(c => c.Centro)
                .Include(c => c.CitAprCodFkNavigation.Ficha)
                    .ThenInclude(c => c.programaFormacion)
                        .ThenInclude(c => c.NivelFormacion)
                .Include(c => c.CitAprCodFkNavigation.Ficha)
                    .ThenInclude(c => c.programaFormacion)
                        .ThenInclude(c => c.Area)
                .Include(c => c.CitPsiCodFkNavigation)
                .Where(c => c.CitEstadoRegistro == "activo" &&
                            c.CitAprCodFkNavigation != null &&
                            c.CitAprCodFkNavigation.Aprendiz != null &&
                            c.CitAprCodFkNavigation.Aprendiz.AprCodigo == aprendizId)
                .OrderByDescending(c => c.CitFechaCreacion)
                .ToListAsync();

            var resultados = datos.Select(MapearCita);
            return Ok(resultados);
        }


        [Authorize(Roles = Roles.Aprendiz)]
        [HttpPost("solicitar-cita")]
        public async Task<IActionResult> solicitarCita([FromBody] SolicitudCitaDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("El cuerpo de la solicitud no debe estar vacio!");
            }

            if (!TryObtenerAprendizIdAutenticado(out var aprendizId))
            {
                return Forbid();
            }

            if (!EsTipoCitaValido(dto.TipoCita))
            {
                return BadRequest("El tipo de cita no es válido.");
            }

            if (string.IsNullOrWhiteSpace(dto.MotivoSolicitud))
            {
                return BadRequest("El motivo de la solicitud es obligatorio.");
            }

            var datosAprendiz = await ObtenerAprendizFichaActivaAsync(aprendizId);
            var psicologoAsignado = datosAprendiz?.Ficha
                        .programaFormacion
                        .Area
                        .AreaPsicologo;
            
            if (datosAprendiz == null)
                return NotFound("El aprendiz no tiene una ficha activa asociada.");

            if (datosAprendiz.Aprendiz == null)
                return BadRequest("El aprendiz existe en AprendizFicha pero no tiene datos en la tabla Aprendiz.");

            if (datosAprendiz.Aprendiz.AprEstadoRegistro != "activo")
                return BadRequest("El aprendiz debe estar activo para solicitar una cita.");

            if (datosAprendiz.Ficha == null)
                return BadRequest("El aprendiz no tiene ficha asociada.");

            if (datosAprendiz.Ficha.programaFormacion == null)
                return BadRequest("La ficha del aprendiz no tiene un programa de formación asociado.");

            if (datosAprendiz.Ficha.programaFormacion.Area == null)
                return BadRequest("El programa de formación no tiene un área asociada.");

            if (datosAprendiz.Ficha.programaFormacion.Area.AreaPsicologo == null)
                return BadRequest("El área del programa no tiene un psicólogo asignado.");

            var solicitudActiva = await _uow.Citas.Query()
                .Where(c => c.CitEstadoRegistro == "activo" &&
                            c.CitAprCodFk == datosAprendiz.AprFicCodigo &&
                            c.CitEstadoCita != null &&
                            EstadosSolicitudActiva.Contains(c.CitEstadoCita))
                .AnyAsync();

            if (solicitudActiva)
            {
                return BadRequest("Ya tienes una solicitud o cita activa pendiente de gestión.");
            }

            
            var soli = new Cita
            {
                CitTipoCita = dto.TipoCita.Trim().ToLower(),
                CitFechaCreacion = DateTime.UtcNow,
                CitMotivoSolicitud = dto.MotivoSolicitud.Trim(),
                CitEstadoCita = EstadoPendiente,
                CitAprCodFk = datosAprendiz.AprFicCodigo,
                CitPsiCodFk = psicologoAsignado.PsiCodigo
            };
            await _uow.Citas.Agregar(soli);
            await _uow.SaveChangesAsync();

            var mensaje = "Se ha solicitado una cita correctamente";
            var nombreCompleto = $"{datosAprendiz.Aprendiz.AprNombre} {datosAprendiz.Aprendiz.AprApellido}".Trim();
            if (string.IsNullOrWhiteSpace(nombreCompleto))
                nombreCompleto = "Un aprendiz";

            var notifTitle = "Nueva solicitud de cita";
            var notifMessage =
                $"{nombreCompleto} — Ficha {datosAprendiz.Ficha.FicCodigo}. Tipo: {dto.TipoCita.Trim()}.";

            try
            {
                await _chatPush.NotifyPsychologistAsync(
                    psicologoAsignado.PsiCodigo,
                    "CITA_SOLICITADA",
                    notifTitle,
                    notifMessage,
                    soli.CitCodigo);
            }
            catch (Exception exPush)
            {
                _ = exPush;
            }

            var correoInstitucional = psicologoAsignado.PsiCorreoInstitucional?.Trim();
            if (!string.IsNullOrEmpty(correoInstitucional))
            {
                try
                {
                    var html = BuildSolicitudCitaEmailHtml(
                        nombreCompleto,
                        datosAprendiz.Ficha.FicCodigo,
                        dto.TipoCita.Trim(),
                        dto.MotivoSolicitud.Trim(),
                        soli.CitCodigo,
                        soli.CitFechaCreacion ?? DateTime.UtcNow);
                    await _emailService.SendAsync(
                        correoInstitucional,
                        "[Healthy Mind] Nueva solicitud de cita",
                        html,
                        isHtml: true);
                }
                catch
                {
                    mensaje = "La solicitud se creó correctamente, pero no se pudo enviar el correo al psicólogo. Revisa la configuración SMTP o el buzón institucional.";
                }
            }

            return Ok(new
            {
                mensaje,
                citaId = soli.CitCodigo,
                estado = soli.CitEstadoCita,
                psicologoId = psicologoAsignado.PsiCodigo
            });


        }

        private string BuildSolicitudCitaEmailHtml(
            string nombreAprendiz,
            int fichaCodigo,
            string tipoCita,
            string motivoSolicitud,
            int citaCodigo,
            DateTime fechaUtc)
        {
            var esc = new Func<string?, string>(s => WebUtility.HtmlEncode(s ?? ""));
            var portal = (_configuration["App:PsychologistPortalUrl"] ?? "").Trim();
            var linkBlock = new StringBuilder();
            if (!string.IsNullOrEmpty(portal))
            {
                var safeUrl = esc(portal);
                linkBlock.Append(
                    $"<p style=\"margin:16px 0 0 0;\"><a href=\"{safeUrl}\" style=\"color:#5b21b6;font-weight:600;\">Abrir panel de citas en Healthy Mind</a></p>");
            }

            var fechaLocal = fechaUtc.ToString("yyyy-MM-dd HH:mm") + " UTC";
            return $@"<!DOCTYPE html><html><head><meta charset=""utf-8"" /></head>
<body style=""font-family:Segoe UI,Roboto,sans-serif;font-size:15px;color:#1e293b;line-height:1.5;"">
<p>Hola,</p>
<p><strong>{esc(nombreAprendiz)}</strong> ha enviado una <strong>nueva solicitud de cita</strong> en la plataforma Healthy Mind.</p>
<table style=""border-collapse:collapse;margin:12px 0;"" cellpadding=""0"" cellspacing=""0"">
<tr><td style=""padding:4px 12px 4px 0;color:#64748b;"">Ficha</td><td><strong>#{fichaCodigo}</strong></td></tr>
<tr><td style=""padding:4px 12px 4px 0;color:#64748b;"">Tipo solicitado</td><td><strong>{esc(tipoCita)}</strong></td></tr>
<tr><td style=""padding:4px 12px 4px 0;color:#64748b;vertical-align:top;"">Motivo</td><td>{esc(motivoSolicitud)}</td></tr>
<tr><td style=""padding:4px 12px 4px 0;color:#64748b;"">Nº solicitud</td><td><strong>{citaCodigo}</strong></td></tr>
<tr><td style=""padding:4px 12px 4px 0;color:#64748b;"">Registrada</td><td>{esc(fechaLocal)}</td></tr>
</table>
<p style=""margin-top:16px;color:#64748b;font-size:13px;"">Gestiona esta solicitud en <strong>Solicitudes pendientes</strong> dentro de Citas.</p>
{linkBlock}
<hr style=""border:none;border-top:1px solid #e2e8f0;margin:24px 0;"" />
<p style=""font-size:12px;color:#94a3b8;"">Mensaje automático. No respondas a este correo.</p>
</body></html>";
        }

        [Authorize(Roles = Roles.Aprendiz)]
        [HttpPut("cancelar-mi-solicitud/{id}")]
        public async Task<IActionResult> CancelarMiSolicitud(int id)
        {
            if (!TryObtenerAprendizIdAutenticado(out var aprendizId))
            {
                return Forbid();
            }

            var aprFicCodigo = await _uow.AprendizFicha.Query()
                .Where(a => a.Aprendiz != null &&
                            a.Aprendiz.AprCodigo == aprendizId &&
                            a.AprFicEstadoRegistro == "activo")
                .Select(a => (int?)a.AprFicCodigo)
                .FirstOrDefaultAsync();

            if (!aprFicCodigo.HasValue)
            {
                return NotFound("El aprendiz no tiene una ficha activa asociada.");
            }

            var resultado = await _uow.Citas.Query()
                .Where(a => a.CitCodigo == id &&
                            a.CitEstadoRegistro == "activo" &&
                            a.CitEstadoCita == EstadoPendiente &&
                            a.CitAprCodFk == aprFicCodigo.Value)
                .FirstOrDefaultAsync();

            if (resultado == null)
            {
                return NotFound("No se encontró una solicitud pendiente con ese id.");
            }

            resultado.CitEstadoCita = EstadoCancelada;

            _uow.Citas.Actualizar(resultado);
            await _uow.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "La solicitud fue cancelada correctamente.",
                citaId = resultado.CitCodigo,
                estado = resultado.CitEstadoCita
            });
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("cancelar-cita/{id}")]
        public async Task<IActionResult> CancelarCita(int id)
        {
            var regEncontrado = await _uow.Citas.ObtenerTodoConCondicion(a => a.CitCodigo == id
            && a.CitEstadoCita == "pendiente" && a.CitEstadoRegistro == "activo");

            if (!regEncontrado.Any())
            {
                return NotFound("No se encontró esta cita");
            }

            var resultado = regEncontrado.FirstOrDefault();
            resultado.CitEstadoCita = EstadoCancelada;

            _uow.Citas.Actualizar(resultado);
            await _uow.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Se han editado correctamente los datos!",
                resultado
            });
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("editar-solicitudes/{id}")]
        public async Task<IActionResult> EditarInformacion(int id, [FromBody] CitaDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "El cuerpo no debe estar vacio!" });
            }

            var regEncontrado = await _uow.Citas.ObtenerTodoConCondicion(a => a.CitCodigo == id 
            && a.CitEstadoCita == EstadoPendiente && a.CitEstadoRegistro == "activo");

            if (!regEncontrado.Any())
            {
                return NotFound(new { message = "No se encontró una solicitud pendiente con ese id." });
            }

            var resultado = regEncontrado.FirstOrDefault();

            var errorValidacion = ValidarDatosAgenda(dto);
            if (errorValidacion != null)
            {
                return BadRequest(new { message = errorValidacion });
            }

            resultado.CitTipoCita = dto.CitTipoCita!.Trim().ToLower();
            resultado.CitFechaProgramada = dto.CitFechaProgramada;
            resultado.CitHoraInicio = dto.CitHoraInicio;
            resultado.CitHoraFin = dto.CitHoraFin;
            resultado.CitMotivo = dto.CitMotivo;
            resultado.CitAnotaciones = dto.CitAnotaciones;
            resultado.CitEstadoCita = dto.CitEstadoCita!.Trim().ToLower();



            _uow.Citas.Actualizar(resultado);
            await _uow.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Se han editado correctamente los datos!",
                resultado
            });
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("editar/{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] CitaDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("El cuerpo no debe estar vacio!");
            }

            var regEncontrado = await _uow.Citas.ObtenerTodoConCondicion(a => a.CitCodigo == id && a.CitEstadoRegistro == "activo");

            if (!regEncontrado.Any())
            {
                return NotFound("No se encontró esta cita.");
            }

            var resultado = regEncontrado.FirstOrDefault();

            var errorValidacion = ValidarDatosAgenda(dto);
            if (errorValidacion != null)
            {
                return BadRequest(errorValidacion);
            }

            resultado.CitTipoCita = dto.CitTipoCita!.Trim().ToLower();
            resultado.CitFechaProgramada = dto.CitFechaProgramada;
            resultado.CitHoraInicio = dto.CitHoraInicio;
            resultado.CitHoraFin = dto.CitHoraFin;
            resultado.CitMotivo = dto.CitMotivo;
            resultado.CitAnotaciones = dto.CitAnotaciones;
            resultado.CitEstadoCita = dto.CitEstadoCita!.Trim().ToLower();



            _uow.Citas.Actualizar(resultado);
            await _uow.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Se han editado correctamente los datos!",
                resultado
            });
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("cambiar-estado-registro/{id}")]
        public async Task<IActionResult> EliminarRegistro(int id)
        {
            var regEncontrado = (await _uow.Citas.ObtenerTodoConCondicion(a => a.CitCodigo == id && a.CitEstadoRegistro == "activo")).FirstOrDefault();

            if (regEncontrado == null)
                return NotFound("No se encontró este id.");

            regEncontrado.CitEstadoRegistro = "inactivo";

            _uow.Citas.Actualizar(regEncontrado);
            await _uow.SaveChangesAsync();
            return Ok("Se ha eliminado correctamente ");
        }

        /// <summary>
        /// Endpoint de diagnóstico: envía una notificación push de prueba al psicólogo indicado
        /// y devuelve el resultado detallado (éxito, fallo HTTP, excepción, config faltante).
        /// Solo accesible para Administradores.
        /// </summary>
        [Authorize(Policy = "SoloAdministrador")]
        [HttpGet("test-push/{psicologoId}")]
        public async Task<IActionResult> TestPush(int psicologoId)
        {
            if (psicologoId <= 0)
                return BadRequest("psicologoId debe ser mayor que 0.");

            var baseUrl = (_configuration["Chat:NotifyBaseUrl"] ?? "").Trim();
            var secretPresent = !string.IsNullOrWhiteSpace(
                _configuration["Chat:InternalNotifySecret"]
                ?? Environment.GetEnvironmentVariable("CHAT_INTERNAL_NOTIFY_SECRET"));

            var diagnostics = new
            {
                chatNotifyBaseUrl = string.IsNullOrWhiteSpace(baseUrl) ? "(NO CONFIGURADO)" : baseUrl,
                internalNotifySecretPresent = secretPresent,
                targetRoom = $"Psicologo_{psicologoId}",
                timestamp = DateTime.UtcNow
            };

            if (string.IsNullOrWhiteSpace(baseUrl) || !secretPresent)
            {
                return Ok(new
                {
                    resultado = "OMITIDO",
                    motivo = "Chat:NotifyBaseUrl o Chat:InternalNotifySecret no están configurados en la API.",
                    diagnostics
                });
            }

            try
            {
                await _chatPush.NotifyPsychologistAsync(
                    psicologoId,
                    "TEST_PUSH",
                    "Notificación de prueba",
                    $"Push de diagnóstico enviado a Psicologo_{psicologoId} a las {DateTime.UtcNow:HH:mm:ss} UTC.",
                    cancellationToken: HttpContext.RequestAborted);

                return Ok(new
                {
                    resultado = "ENVIADO",
                    motivo = "El push se ejecutó sin excepciones. Revisa la consola del navegador del psicólogo (F12 → [HM-Notif]) y los logs del Chat en Render.",
                    diagnostics
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    resultado = "EXCEPCION",
                    motivo = ex.Message,
                    innerException = ex.InnerException?.Message,
                    diagnostics
                });
            }
        }


    }
}
