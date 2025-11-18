using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using static API_healthyMind.Controllers.AprendizController;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeguimientoAprendizController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;
        


        public SeguimientoAprendizController(IUnidadDeTrabajo uow)
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


        // GET: NivelFormacionController
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.SeguimientoAprendiz.ObtenerTodoConCondicion(e => e.SegEstadoRegistro == "activo",
                e => e.Include(c => c.SegAprendizFkNavigation)
                    .ThenInclude(c => c.Aprendiz)
                        .ThenInclude(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                      .Include(c => c.SegAprendizFkNavigation.Aprendiz.EstadoAprendiz)
                      .Include(c => c.SegAprendizFkNavigation.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.SegAprendizFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.SegAprendizFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.SegPsicologoFkNavigation)
                        );
                            
                       

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            var resultados = datos.Select(c => new
            {
                c.SegCodigo,
                aprendiz = MapearAprendizFicha(c.SegAprendizFkNavigation),
                psicologo = c.SegPsicologoFkNavigation,
                FechaInicioSeguimiento = c.SegFechaSeguimiento,
                FechaFinSeguimiento = c.SegFechaFin,
                AreaRemitido = c.SegAreaRemitido,
                TrimestreActual = c.SegTrimestreActual,
                Motivo = c.SegMotivo,
                Descripcion = c.SegDescripcion,
                Recomendaciones = c.SegRecomendaciones,
                FirmaProfesional = c.SegFirmaProfesional,
                FirmaAprendiz = c.SegFirmaAprendiz
            });

            return Ok(resultados);
        }

        [HttpGet("listar")]
        public async Task<IActionResult> ListarSeguimientos([FromQuery] PaginacionDTO p)
        {
            if (p.TamanoPagina > 100) // límite de seguridad opcional
                p.TamanoPagina = 100;

            var query = _uow.SeguimientoAprendiz.Query()
                        .Include(c => c.SegAprendizFkNavigation)
                            .ThenInclude(c => c.Aprendiz)
                                .ThenInclude(c => c.Municipio)
                                    .ThenInclude(c => c.Regional)
                        .Include(c => c.SegAprendizFkNavigation.Aprendiz.EstadoAprendiz)
                        .Include(c => c.SegAprendizFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                               .ThenInclude(c => c.Centro)
                        .Include(c => c.SegAprendizFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.SegAprendizFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.SegPsicologoFkNavigation)
                        .Where(c => c.SegEstadoRegistro == "activo"); // Orden para paginar estable

            var totalRegistros = await query.CountAsync();

            var datos = await query
                .Skip((p.Pagina - 1) * p.TamanoPagina)
                .Take(p.TamanoPagina)
                .ToListAsync();

            var resultados = datos.Select(c => new
            {
                c.SegCodigo,
                aprendiz = MapearAprendizFicha(c.SegAprendizFkNavigation),
                psicologo = c.SegPsicologoFkNavigation,
                FechaInicioSeguimiento = c.SegFechaSeguimiento,
                FechaFinSeguimiento = c.SegFechaFin,
                AreaRemitido = c.SegAreaRemitido,
                TrimestreActual = c.SegTrimestreActual,
                Motivo = c.SegMotivo,
                Descripcion = c.SegDescripcion,
                Recomendaciones = c.SegRecomendaciones,
                FirmaProfesional = c.SegFirmaProfesional,
                FirmaAprendiz = c.SegFirmaAprendiz
            });

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
        public async Task<IActionResult> Buscar([FromQuery] FiltroAprendizFichaDTO f)
        {
            IQueryable<SeguimientoAprendiz> q = _uow.SeguimientoAprendiz.Query()
                .Include(c => c.SegAprendizFkNavigation)
                    .ThenInclude(c => c.Aprendiz)
                        .ThenInclude(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                      .Include(c => c.SegAprendizFkNavigation.Aprendiz.EstadoAprendiz)
                      .Include(c => c.SegAprendizFkNavigation.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.SegAprendizFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.SegAprendizFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.SegPsicologoFkNavigation);

            // ======================
            //    FILTROS
            // ======================

            if (f.FichaCodigo.HasValue)
                q = q.Where(x => x.SegAprendizFkNavigation.Ficha.FicCodigo == f.FichaCodigo.Value);

            if (!string.IsNullOrEmpty(f.AreaNombre))
                q = q.Where(x => x.SegAprendizFkNavigation.Ficha.programaFormacion.Area.AreaNombre.ToLower()
                    .Contains(f.AreaNombre.ToLower()));

            if (!string.IsNullOrEmpty(f.ProgramaNombre))
                q = q.Where(x => x.SegAprendizFkNavigation.Ficha.programaFormacion.ProgNombre.ToLower()
                    .Contains(f.ProgramaNombre.ToLower()));

            if (f.PsicologoID.HasValue)
                q = q.Where(x => x.SegPsicologoFkNavigation.PsiDocumento == f.PsicologoID.Value);

            if (!string.IsNullOrEmpty(f.TipoPoblacion))
                q = q.Where(x => x.SegAprendizFkNavigation.Aprendiz.AprTipoPoblacion.ToLower() == f.TipoPoblacion.ToLower());

            if (!string.IsNullOrEmpty(f.Eps))
                q = q.Where(x => x.SegAprendizFkNavigation.Aprendiz.AprEps.ToLower() == f.Eps.ToLower());

            if (f.EstadoAprendizID.HasValue)
                q = q.Where(x => x.SegAprendizFkNavigation.Aprendiz.EstadoAprendiz.EstAprCodigo == f.EstadoAprendizID.Value);

            if (!string.IsNullOrEmpty(f.CentroNombre))
                q = q.Where(x => x.SegAprendizFkNavigation.Ficha.programaFormacion.Centro.CenNombre.ToLower()
                    .Contains(f.CentroNombre.ToLower()));

            if (!string.IsNullOrEmpty(f.Jornada))
                q = q.Where(x => x.SegAprendizFkNavigation.Ficha.FicJornada.ToLower() == f.Jornada.ToLower());

            if (!string.IsNullOrEmpty(f.AreaRemitido))
                q = q.Where(x => x.SegAreaRemitido.ToLower() == f.AreaRemitido.ToLower());

            if (f.TrimestreActual.HasValue)
                q = q.Where(x => x.SegTrimestreActual == f.TrimestreActual.Value);

            if (f.FechaInicioDesde.HasValue)
            {
                var desde = f.FechaInicioDesde.Value.Date;
                q = q.Where(x => x.SegFechaSeguimiento.HasValue && x.SegFechaSeguimiento.Value.Date >= desde);
            }

            if (f.FechaInicioHasta.HasValue)
            {
                var hasta = f.FechaInicioHasta.Value.Date;
                q = q.Where(x => x.SegFechaSeguimiento.HasValue && x.SegFechaSeguimiento.Value.Date <= hasta);
            }

            if (f.FechaFinDesde.HasValue)
            {
                var desdeFin = f.FechaFinDesde.Value.Date;
                q = q.Where(x => x.SegFechaFin.HasValue && x.SegFechaFin.Value.Date >= desdeFin);
            }

            if (f.FechaFinHasta.HasValue)
            {
                var hastaFin = f.FechaFinHasta.Value.Date;
                q = q.Where(x => x.SegFechaFin.HasValue && x.SegFechaFin.Value.Date <= hastaFin);
            }

            q = q.Where(x => x.SegEstadoRegistro.ToLower() == "activo");


            var datos = await q.ToListAsync();

            if (!datos.Any())
                return NotFound("No se encontraron resultados con esos filtros.");

            var resultado = datos.Select(c => new
            {
                c.SegCodigo,
                aprendiz = MapearAprendizFicha(c.SegAprendizFkNavigation),
                psicologo = c.SegPsicologoFkNavigation,
                FechaInicioSeguimiento = c.SegFechaSeguimiento,
                FechaFinSeguimiento = c.SegFechaFin,
                AreaRemitido = c.SegAreaRemitido,
                TrimestreActual = c.SegTrimestreActual,
                Motivo = c.SegMotivo,
                Descripcion = c.SegDescripcion,
                Recomendaciones = c.SegRecomendaciones,
                FirmaProfesional = c.SegFirmaProfesional,
                FirmaAprendiz = c.SegFirmaAprendiz
            }); 

            return Ok(resultado);
        }



        [HttpGet("estadistica/por-mes-inicio")]
        public async Task<IActionResult> GetSeguimientosIniciadosPorMes()
        {
            var resultado = await _uow.SeguimientoAprendiz.Query()
                .Where(x => x.SegEstadoRegistro == "activo")
                .GroupBy(x => new
                {
                    Año = x.SegFechaSeguimiento.Value.Year,
                    Mes = x.SegFechaSeguimiento.Value.Month
                })
                .Select(g => new
                {
                    Año = g.Key.Año,
                    Mes = g.Key.Mes,
                    Total_Seguimientos_Iniciados = g.Count()
                })
                .OrderBy(x => x.Año)
                .ThenBy(x => x.Mes)
                .ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("estadistica/por-mes-fin")]
        public async Task<IActionResult> GetSeguimientosFinalizadosPorMes()
        {
            var resultado = await _uow.SeguimientoAprendiz.Query()
                .Where(x => x.SegEstadoRegistro == "activo")
                .GroupBy(x => new
                {
                    Año = x.SegFechaFin.Value.Year,
                    Mes = x.SegFechaFin.Value.Month
                })
                .Select(g => new
                {
                    Año = g.Key.Año,
                    Mes = g.Key.Mes,
                    Total_Seguimientos_Finalizados = g.Count()
                })
                .OrderBy(x => x.Año)
                .ThenBy(x => x.Mes)
                .ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("estadistica/por-dia-inicio")]
        public async Task<IActionResult> GetSeguimientosIniciadosPorDia()
        {
            var resultado = await _uow.SeguimientoAprendiz.Query()
                .Where(x => x.SegEstadoRegistro == "activo" && x.SegFechaSeguimiento != null)
                .GroupBy(x => new
                {
                    Año = x.SegFechaSeguimiento.Value.Year,
                    Mes = x.SegFechaSeguimiento.Value.Month,
                    Dia = x.SegFechaSeguimiento.Value.Day
                })
                .Select(g => new
                {
                    Año = g.Key.Año,
                    Mes = g.Key.Mes,
                    Dia = g.Key.Dia,
                    Total_Seguimientos_Iniciados = g.Count()
                })
                .OrderBy(x => x.Año)
                .ThenBy(x => x.Mes)
                .ThenBy(x => x.Dia)
                .ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("estadistica/por-dia-fin")]
        public async Task<IActionResult> GetSeguimientosFinalizadosPorDia()
        {
            var resultado = await _uow.SeguimientoAprendiz.Query()
                .Where(x => x.SegEstadoRegistro == "activo" && x.SegFechaFin != null)
                .GroupBy(x => new
                {
                    Año = x.SegFechaFin.Value.Year,
                    Mes = x.SegFechaFin.Value.Month,
                    Dia = x.SegFechaFin.Value.Day
                })
                .Select(g => new
                {
                    Año = g.Key.Año,
                    Mes = g.Key.Mes,
                    Dia = g.Key.Dia,
                    Total_Seguimientos_Finalizados = g.Count()
                })
                .OrderBy(x => x.Año)
                .ThenBy(x => x.Mes)
                .ThenBy(x => x.Dia)
                .ToListAsync();

            return Ok(resultado);
        }



        [HttpPost]
        public async Task<IActionResult> CrearRegistro(SeguimientoAprendizDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("El cuerpo no debe estar vacio!");
            }

            var nuevoReg = new SeguimientoAprendiz
            {
                SegAprendizFk = dto.SegAprendizFk,
                SegPsicologoFk = dto.SegPsicologoFk,
                SegFechaSeguimiento = dto.SegFechaSeguimiento,
                SegFechaFin = dto.SegFechaFin,
                SegAreaRemitido = dto.SegAreaRemitido,
                SegTrimestreActual = dto.SegTrimestreActual,
                SegMotivo = dto.SegMotivo,
                SegDescripcion = dto.SegDescripcion,
                SegRecomendaciones = dto.SegRecomendaciones,
                SegFirmaProfesional = dto.SegFirmaProfesional,
                SegFirmaAprendiz = dto.SegFirmaAprendiz
            };

            await _uow.SeguimientoAprendiz.Agregar(nuevoReg);
            await _uow.SaveChangesAsync();
            return Ok(new
            {
                mensaje = "Se ha creado el registro exitosamente",
                nuevoReg
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
