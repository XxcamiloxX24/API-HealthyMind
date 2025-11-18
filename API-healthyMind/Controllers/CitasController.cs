using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Models.DTO.Filtros;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using static API_healthyMind.Controllers.AprendizController;
namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitasController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;
        


        public CitasController(IUnidadDeTrabajo uow)
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
                Ubicacion = new
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
                    ProgramaFormacion = new
                    {
                        d.Ficha.programaFormacion.ProgCodigo,
                        d.Ficha.programaFormacion.ProgNombre,
                        d.Ficha.programaFormacion.ProgModalidad,
                        d.Ficha.programaFormacion.ProgFormaModalidad,
                        d.Ficha.programaFormacion.NivelFormacion,
                        Area = new {
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
                        .Include(c => c.CitPsiCodFkNavigation);

            var totalRegistros = await query.CountAsync();

            var datos = await query
                .Skip((p.Pagina - 1) * p.TamanoPagina)
                .Take(p.TamanoPagina)
                .ToListAsync();

            var resultados = datos.Select(c => MapearCita(c));

            return Ok(new
            {
                paginaActual = p.Pagina,
                tamanoPagina = p.TamanoPagina,
                totalRegistros,
                totalPaginas = (int)Math.Ceiling(totalRegistros / (double)p.TamanoPagina),
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

            var datos = await query
                .Skip((p.Pagina - 1) * p.TamanoPagina)
                .Take(p.TamanoPagina)
                .ToListAsync();

            var resultados = datos.Select(c => MapearCita(c));

            return Ok(new
            {
                paginaActual = p.Pagina,
                tamanoPagina = p.TamanoPagina,
                totalRegistros,
                totalPaginas = (int)Math.Ceiling(totalRegistros / (double)p.TamanoPagina),
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

            if (f.PsicologoDocumento.HasValue)
                q = q.Where(x => x.CitPsiCodFkNavigation.PsiDocumento == f.PsicologoDocumento.Value);

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
                q = q.Where(x => x.CitTipoCita == f.TipoCita.ToLower());
            

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
                .Where(x => x.CitEstadoRegistro == "activo")
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

        [HttpGet("estadistica/por-dia")]
        public async Task<IActionResult> GetSeguimientosFinalizadosPorMes()
        {
            var resultado = await _uow.Citas.Query()
                .Where(x => x.CitEstadoRegistro == "activo")
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
            return Ok(new
            {
                mensaje = "Se ha creado el registro exitosamente",
                nuevoReg
            });
        }

        [HttpPost("solicitar-cita")]
        public async Task<IActionResult> solicitarCita([FromBody] SolicitudCitaDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("El cuerpo de la solicitud no debe estar vacio!");
            }

            var datosAprendiz = await _uow.AprendizFicha.Query()
                                            .Include(x => x.Ficha)
                                                .ThenInclude(f => f.programaFormacion)
                                                    .ThenInclude(p => p.Area)
                                                        .ThenInclude(a => a.AreaPsicologo) // relación área → psicólogo
                                            .Where(x => x.Aprendiz.AprNroDocumento == dto.NroDocumento)
                                            .FirstOrDefaultAsync();
            var psicologoAsignado = datosAprendiz?.Ficha
                        .programaFormacion
                        .Area
                        .AreaPsicologo;
            var soli = new Cita
            {
                CitFechaCreacion = DateTime.Now,
                CitMotivoSolicitud = dto.MotivoSolicitud,
                CitEstadoCita = "pendiente",
                CitAprCodFk = datosAprendiz.Aprendiz.AprCodigo,
                CitPsiCodFk = psicologoAsignado.PsiCodigo
            };
            await _uow.Citas.Agregar(soli);
            await _uow.SaveChangesAsync();

            return Ok(new
            {
                Mensaje = "Se ha solicitado una cita correctamente",
                soli
            });


        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarInformacion(int id, [FromBody] SeguimientoAprendizDTO dto)
        {
            var regEncontrado = await _uow.SeguimientoAprendiz.ObtenerTodoConCondicion(a => a.SegCodigo == id 
            && a.SegEstadoRegistro == "activo");

            if (!regEncontrado.Any())
            {
                return NotFound("No se encontró este aprendiz");
            }

            var resultado = regEncontrado.FirstOrDefault();


            resultado.SegAprendizFk = dto.SegAprendizFk;
            resultado.SegPsicologoFk = dto.SegPsicologoFk;
            resultado.SegFechaSeguimiento = dto.SegFechaSeguimiento;
            resultado.SegFechaFin = dto.SegFechaFin;
            resultado.SegAreaRemitido = dto.SegAreaRemitido;
            resultado.SegTrimestreActual = dto.SegTrimestreActual;
            resultado.SegMotivo = dto.SegMotivo;
            resultado.SegDescripcion = dto.SegDescripcion;
            resultado.SegRecomendaciones = dto.SegRecomendaciones;
            resultado.SegFirmaProfesional = dto.SegFirmaProfesional;
            resultado.SegFirmaAprendiz = dto.SegFirmaAprendiz;



            _uow.SeguimientoAprendiz.Actualizar(resultado);
            await _uow.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Se han editado correctamente los datos!",
                resultado
            });
        }


        [HttpPut("eliminar/{id}")]
        public async Task<IActionResult> EliminarRegistro(int id)
        {
            var regEncontrado = (await _uow.SeguimientoAprendiz.ObtenerTodoConCondicion(a => a.SegCodigo == id && a.SegEstadoRegistro == "activo")).FirstOrDefault();

            if (regEncontrado == null)
                return NotFound("No se encontró este id.");

            regEncontrado.SegEstadoRegistro = "inactivo";

            _uow.SeguimientoAprendiz.Actualizar(regEncontrado);
            await _uow.SaveChangesAsync();
            return Ok("Se ha eliminado correctamente ");


        }

    }
}
