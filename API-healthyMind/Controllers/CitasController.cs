using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Models.DTO.Filtros;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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


        public CitasController(IUnidadDeTrabajo uow, IEmailService emailService)
        {
            _uow = uow;
            _emailService = emailService;
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
                c.CitMotivo,
                c.CitAnotaciones,
                c.CitEstadoCita,
                aprendizCita = MapearAprendizFicha(c.CitAprCodFkNavigation),
                psicologo = c.CitPsiCodFkNavigation,
            };
        }


        // GET: NivelFormacionController
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



        [HttpGet("estadistica/por-mes")]
        public async Task<IActionResult> GetSeguimientosIniciadosPorMes()
        {
            var resultado = await _uow.Citas.Query()
                .Where(x => x.CitEstadoRegistro == "activo" &&
                    x.CitEstadoCita != "pendiente" &&
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

        [HttpGet("estadistica/actividad-exitosa")]
        public async Task<IActionResult> GetActividadExitosa()
        {
            var total = await _uow.Citas.Query().CountAsync();
            var exitosas = await _uow.Citas.Query().CountAsync(c => c.CitEstadoCita == "Realizada");

            double porcentaje = total == 0 ? 0 : (double)exitosas / total * 100;

            return Ok(new
            {
                totalCitas = total,
                exitosas,
                porcentaje = Math.Round(porcentaje, 2)
            });
        }

        [HttpGet("citas/estado-proceso")]
        public async Task<IActionResult> GetCitasEnProceso()
        {
            // Estados considerados "en proceso"
            var estadosProceso = new[] { "pendiente", "programada", "reprogramada" };

            // Total de citas
            var totalCitas = await _uow.Citas.Query().CountAsync();

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
                .Where(c => estadosProceso.Contains(c.CitEstadoCita))
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


        [HttpGet("citas/estado-incidencias")]
        public async Task<IActionResult> GetCitasincidencias()
        {
            // Estados considerados "en proceso"
            var estadosProceso = new[] { "cancelada", "no asistió" };

            // Total de citas
            var totalCitas = await _uow.Citas.Query().CountAsync();

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
                .Where(c => estadosProceso.Contains(c.CitEstadoCita))
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

        [HttpGet("estadistica/por-dia")]
        public async Task<IActionResult> GetSeguimientosFinalizadosPorMes()
        {
            var resultado = await _uow.Citas.Query()
                .Where(x => x.CitEstadoRegistro == "activo" &&
                    x.CitEstadoCita != "pendiente" &&
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



        [HttpPost]
        public async Task<IActionResult> CrearRegistro(CitaDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("El cuerpo no debe estar vacio!");
            }

            var nuevoReg = new Cita
            {
                CitTipoCita = dto.CitTipoCita,
                CitFechaProgramada = dto.CitFechaProgramada,
                CitHoraInicio = dto.CitHoraInicio,
                CitHoraFin = dto.CitHoraFin,
                CitMotivo = dto.CitMotivo,
                CitAnotaciones = dto.CitAnotaciones,
                CitFechaCreacion = DateTime.Now,
                CitEstadoCita = dto.CitEstadoCita,
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


        [HttpPost("solicitar-cita")]
        public async Task<IActionResult> solicitarCita([FromBody] SolicitudCitaDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("El cuerpo de la solicitud no debe estar vacio!");
            }

            var datosAprendiz = await _uow.AprendizFicha.Query()
                                            .Include(c => c.Aprendiz)
                                                .ThenInclude(c => c.EstadoAprendiz)
                                            .Include(c => c.Aprendiz)
                                                .ThenInclude(c => c.Municipio)
                                                    .ThenInclude(c => c.Regional)
                                            .Include(x => x.Ficha)
                                                .ThenInclude(f => f.programaFormacion)
                                                    .ThenInclude(p => p.Area)
                                                        .ThenInclude(a => a.AreaPsicologo)
                                            .Where(x => x.Aprendiz.AprNroDocumento == dto.NroDocumento)
                                            .FirstOrDefaultAsync();
            var psicologoAsignado = datosAprendiz?.Ficha
                        .programaFormacion
                        .Area
                        .AreaPsicologo;
            
            if (datosAprendiz == null)
                return NotFound("No existe ningún registro de AprendizFicha para el aprendiz con ese documento.");

            if (datosAprendiz.Aprendiz == null)
                return BadRequest("El aprendiz existe en AprendizFicha pero no tiene datos en la tabla Aprendiz.");

            if (datosAprendiz.Ficha == null)
                return BadRequest("El aprendiz no tiene ficha asociada.");

            if (datosAprendiz.Ficha.programaFormacion == null)
                return BadRequest("La ficha del aprendiz no tiene un programa de formación asociado.");

            if (datosAprendiz.Ficha.programaFormacion.Area == null)
                return BadRequest("El programa de formación no tiene un área asociada.");

            if (datosAprendiz.Ficha.programaFormacion.Area.AreaPsicologo == null)
                return BadRequest("El área del programa no tiene un psicólogo asignado.");

            
            var soli = new Cita
            {
                CitTipoCita = dto.TipoCita,
                CitFechaCreacion = DateTime.Now,
                CitMotivoSolicitud = dto.MotivoSolicitud,
                CitEstadoCita = "pendiente",
                CitAprCodFk = datosAprendiz.Aprendiz.AprCodigo,
                CitPsiCodFk = psicologoAsignado.PsiCodigo
            };
            await _uow.Citas.Agregar(soli);
            await _uow.SaveChangesAsync();
            await _emailService.SendAsync(soli.CitPsiCodFkNavigation.PsiCorreoInstitucional,
                "¡Nueva solicitud de cita! - Healthy Mind",
                $"El aprendiz {soli.CitAprCodFkNavigation.Aprendiz.AprNombre} {soli.CitAprCodFkNavigation.Aprendiz.AprApellido} de la ficha Nro {soli.CitAprCodFkNavigation.Ficha.FicCodigo} ha solicitado una cita!");
            return Ok(new
            {
                Mensaje = "Se ha solicitado una cita correctamente",
                datosAprendiz
            });


        }

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
            resultado.CitEstadoCita = "cancelada";

            _uow.Citas.Actualizar(resultado);
            await _uow.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Se han editado correctamente los datos!",
                resultado
            });
        }

        [HttpPut("editar-solicitudes/{id}")]
        public async Task<IActionResult> EditarInformacion(int id, [FromBody] CitaDTO dto)
        {
            var regEncontrado = await _uow.Citas.ObtenerTodoConCondicion(a => a.CitCodigo == id 
            && a.CitEstadoCita == "pendiente" && a.CitEstadoRegistro == "activo");

            if (!regEncontrado.Any())
            {
                return NotFound("No se encontró este aprendiz");
            }

            var resultado = regEncontrado.FirstOrDefault();

            resultado.CitTipoCita = dto.CitTipoCita;
            resultado.CitFechaProgramada = dto.CitFechaProgramada;
            resultado.CitHoraInicio = dto.CitHoraInicio;
            resultado.CitHoraFin = dto.CitHoraFin;
            resultado.CitMotivo = dto.CitMotivo;
            resultado.CitAnotaciones = dto.CitAnotaciones;
            resultado.CitEstadoCita = dto.CitEstadoCita;



            _uow.Citas.Actualizar(resultado);
            await _uow.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Se han editado correctamente los datos!",
                resultado
            });
        }

        [HttpPut("editar/{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] CitaDTO dto)
        {
            var regEncontrado = await _uow.Citas.ObtenerTodoConCondicion(a => a.CitCodigo == id && a.CitEstadoRegistro == "activo");

            if (!regEncontrado.Any())
            {
                return NotFound("No se encontró este aprendiz");
            }

            var resultado = regEncontrado.FirstOrDefault();

            resultado.CitTipoCita = dto.CitTipoCita;
            resultado.CitFechaProgramada = dto.CitFechaProgramada;
            resultado.CitHoraInicio = dto.CitHoraInicio;
            resultado.CitHoraFin = dto.CitHoraFin;
            resultado.CitMotivo = dto.CitMotivo;
            resultado.CitAnotaciones = dto.CitAnotaciones;
            resultado.CitEstadoCita = dto.CitEstadoCita;



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


    }
}
