using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Models.DTO.Filtros;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CualquierRol")]
    public class TestPreguntasController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public TestPreguntasController(IUnidadDeTrabajo uow)
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
                        Area = new
                        {
                            d.Ficha.programaFormacion.Area.AreaCodigo,
                            d.Ficha.programaFormacion.Area.AreaNombre,
                        },
                        d.Ficha.programaFormacion.Centro
                    }
                }

            };
        }

        private static object MapearTestGeneral(TestGeneral d)
        {
            return new
            {
                d.TestGenCodigo,
                AprendizTest = MapearAprendizFicha(d.TestGenApreFkNavigation),
                Psicologo = d.TestGenPsicoFkNavigation,
                FechaRealizado = d.TestGenFechaRealiz,
                Resultados = d.TestGenResultados,
                Recomendaciones = d.TestGenRecomendacion
            };
        }

        private static object MapearTestPreguntas(TestPreguntas d)
        {
            return new
            {
                TestRegistro = MapearTestGeneral(d.TesRegistroFkNavigation),
                Pregunta = d.TesPregFkNavigation,
                respuesta = d.TesRespFkNavigation
            };
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.TestPreguntas.ObtenerTodoConCondicion(e => e.TesEstadoRegistro == "activo",
                e => e.Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation)
                        .ThenInclude(c => c.Aprendiz)
                            .ThenInclude(c => c.Municipio)
                                .ThenInclude(c => c.Regional)
                        .Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation.Aprendiz.EstadoAprendiz)
                        .Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.TesRegistroFkNavigation.TestGenPsicoFkNavigation)
                        .Include(c => c.TesRespFkNavigation)
                            .ThenInclude(c => c.CategoriaRespuesta)
                        .Include(c => c.TesPregFkNavigation)
                            .ThenInclude(c => c.CategoriaPregunta)
                );
            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            var resultados = datos.Select(c => MapearTestPreguntas(c));


            return Ok(resultados);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] FiltroTestGeneralDTO f)
        {
            IQueryable<TestPreguntas> q = _uow.TestPreguntas.Query()
                .Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation)
                        .ThenInclude(c => c.Aprendiz)
                            .ThenInclude(c => c.Municipio)
                                .ThenInclude(c => c.Regional)
                        .Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation.Aprendiz.EstadoAprendiz)
                        .Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.TesRegistroFkNavigation.TestGenPsicoFkNavigation)
                        .Include(c => c.TesRespFkNavigation)
                            .ThenInclude(c => c.CategoriaRespuesta)
                        .Include(c => c.TesPregFkNavigation)
                            .ThenInclude(c => c.CategoriaPregunta);

            if (f.Codigo.HasValue)
                q = q.Where(x => x.TesRegistroFkNavigation.TestGenApreFkNavigation.Aprendiz.AprCodigo == f.Codigo.Value);

            if (!string.IsNullOrEmpty(f.TipoDocumento))
                q = q.Where(x => x.TesRegistroFkNavigation.TestGenApreFkNavigation.Aprendiz.AprTipoDocumento == f.TipoDocumento);

            if (!string.IsNullOrEmpty(f.NroDocumento))
                q = q.Where(x => x.TesRegistroFkNavigation.TestGenApreFkNavigation.Aprendiz.AprNroDocumento == f.NroDocumento);

            if (!string.IsNullOrEmpty(f.PrimerNombre))
                q = q.Where(x => x.TesRegistroFkNavigation.TestGenApreFkNavigation.Aprendiz.AprNombre.ToLower().Contains(f.PrimerNombre.ToLower()));

            if (!string.IsNullOrEmpty(f.PrimerApellido))
                q = q.Where(x => x.TesRegistroFkNavigation.TestGenApreFkNavigation.Aprendiz.AprApellido.ToLower().Contains(f.PrimerApellido.ToLower()));

            if (!string.IsNullOrEmpty(f.MunicipioNombre))
                q = q.Where(x => x.TesRegistroFkNavigation.TestGenApreFkNavigation.Aprendiz.Municipio.CiuNombre.ToLower() == f.MunicipioNombre.ToLower());

            if (!string.IsNullOrEmpty(f.DepartamentoNombre))
                q = q.Where(x => x.TesRegistroFkNavigation.TestGenApreFkNavigation.Aprendiz.Municipio.Regional.RegNombre.ToLower() == f.DepartamentoNombre.ToLower());

            if (f.EstadoAprendizID.HasValue)
                q = q.Where(x => x.TesRegistroFkNavigation.TestGenApreFkNavigation.Aprendiz.EstadoAprendiz.EstAprCodigo == f.EstadoAprendizID.Value);

            if (!string.IsNullOrEmpty(f.Eps))
                q = q.Where(x => x.TesRegistroFkNavigation.TestGenApreFkNavigation.Aprendiz.AprEps.ToLower() == f.Eps.ToLower());

            if (!string.IsNullOrEmpty(f.TipoPoblacion))
                q = q.Where(x => x.TesRegistroFkNavigation.TestGenApreFkNavigation.Aprendiz.AprTipoPoblacion.ToLower() == f.TipoPoblacion.ToLower());

            if (!string.IsNullOrEmpty(f.PsicologoDocumento))
                q = q.Where(x => x.TesRegistroFkNavigation.TestGenPsicoFkNavigation.PsiDocumento == f.PsicologoDocumento);

            if (f.FechaRealizacionDesde.HasValue)
            {
                var desde = f.FechaRealizacionDesde.Value.Date;
                q = q.Where(x => x.TesRegistroFkNavigation.TestGenFechaRealiz.HasValue && x.TesRegistroFkNavigation.TestGenFechaRealiz.Value.Date >= desde);
            }

            if (f.FechaRealizacionHasta.HasValue)
            {
                var hasta = f.FechaRealizacionHasta.Value.Date;
                q = q.Where(x => x.TesRegistroFkNavigation.TestGenFechaRealiz.HasValue && x.TesRegistroFkNavigation.TestGenFechaRealiz.Value.Date <= hasta);
            }




            var datos = await q.ToListAsync();

            if (!datos.Any())
                return NotFound("No se encontraron resultados con esos filtros.");

            var resultados = datos.Select(MapearTestPreguntas);

            return Ok(resultados);
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPost]
        public async Task<IActionResult> CrearRegistro([FromBody] TestPreguntasDTO nuevoReg)
        {

            if (nuevoReg == null)
            {
                return BadRequest("El cuerpo no puede ser nulo.");
            }

            var RegistroNew = new TestPreguntas
            {
                TesRegistroFk = nuevoReg.TesRegistroFk,
                TesPregFk = nuevoReg.TesPregFk,
                TesRespFk = nuevoReg.TesRespFk
            };
            await _uow.TestPreguntas.Agregar(RegistroNew);
            await _uow.SaveChangesAsync();



            var datos = await _uow.TestPreguntas.ObtenerTodoConCondicion(e => e.TesEstadoRegistro == "activo" && e.TesRegistroFk == RegistroNew.TesRegistroFk,
                e => e.Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation)
                        .ThenInclude(c => c.Aprendiz)
                            .ThenInclude(c => c.Municipio)
                                .ThenInclude(c => c.Regional)
                        .Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation.Aprendiz.EstadoAprendiz)
                        .Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.TesRegistroFkNavigation.TestGenPsicoFkNavigation)
                        .Include(c => c.TesRespFkNavigation)
                            .ThenInclude(c => c.CategoriaRespuesta)
                        .Include(c => c.TesPregFkNavigation)
                            .ThenInclude(c => c.CategoriaPregunta)
                );

            if (datos == null || !datos.Any())
            {
                return NotFound("No se encontró este elemento");
            }

            return Ok(new
            {
                mensaje = "Registro creado correctamente!",
                datos
            });
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("editar/{id}")]
        public async Task<IActionResult> EditarRegistro(int id, [FromBody] TestPreguntasDTO regRecibido)
        {
            if (id.ToString() == "")
            {
                return BadRequest("El ID no debe ser nulo");
            }
            var regEncontrado = (await _uow.TestPreguntas.ObtenerTodoConCondicion(c => c.TesRegistroFk == id)).FirstOrDefault();
            
            if (regEncontrado == null || regEncontrado.TesEstadoRegistro == "inactivo")
            {
                return NotFound("No se encontró este ID");
            }

            regEncontrado.TesRegistroFk = regRecibido.TesRegistroFk;
            regEncontrado.TesPregFk = regRecibido.TesPregFk;
            regEncontrado.TesRespFk = regRecibido.TesRespFk;
            

            _uow.TestPreguntas.Actualizar(regEncontrado);
            await _uow.SaveChangesAsync();

            var datos = await _uow.TestPreguntas.ObtenerTodoConCondicion(e => e.TesEstadoRegistro == "activo" && e.TesRegistroFk == regEncontrado.TesRegistroFk,
                e => e.Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation)
                        .ThenInclude(c => c.Aprendiz)
                            .ThenInclude(c => c.Municipio)
                                .ThenInclude(c => c.Regional)
                        .Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation.Aprendiz.EstadoAprendiz)
                        .Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.TesRegistroFkNavigation.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.TesRegistroFkNavigation.TestGenPsicoFkNavigation)
                        .Include(c => c.TesRespFkNavigation)
                            .ThenInclude(c => c.CategoriaRespuesta)
                        .Include(c => c.TesPregFkNavigation)
                            .ThenInclude(c => c.CategoriaPregunta)
                );

            if (datos == null || !datos.Any())
            {
                return NotFound("No se encontró este elemento");
            }

            return Ok(new
            {
                mensaje = "Registro editado correctamente!",
                datos
            });
        }



        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("eliminar/{id}")]
        public async Task<IActionResult> EliminarRegistro(int idTesGeneral, int idPreg, int idResp)
        {
            var programaEncontrado = (await _uow.TestPreguntas.ObtenerTodoConCondicion(c => c.TesRegistroFk == idTesGeneral && c.TesPregFk == idPreg && c.TesRespFk == idResp)).FirstOrDefault();
            if (programaEncontrado.TesEstadoRegistro == "inactivo" || programaEncontrado == null)
            {
                return NotFound("No se encontró este ID");
            }
            programaEncontrado.TesEstadoRegistro = "inactivo";

            _uow.TestPreguntas.Actualizar(programaEncontrado);
            await _uow.SaveChangesAsync();

            return Ok("Se ha eliminado correctamente!");
        }
        
    }
}
