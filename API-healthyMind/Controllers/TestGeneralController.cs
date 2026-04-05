using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Models.DTO.Filtros;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_healthyMind.Controllers
{
    /// <summary>
    /// Alias sin la palabra "Test" en la ruta: algunos bloqueadores alteran URLs que contienen "TestGeneral".
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/evaluaciones-hm")]
    [Authorize(Policy = "CualquierRol")]
    public class TestGeneralController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public TestGeneralController(IUnidadDeTrabajo uow)
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

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.TestGeneral.ObtenerTodoConCondicion(e => e.TestGenEstado == "activo",
                e => e.Include(c => c.TestGenApreFkNavigation)
                    .ThenInclude(c => c.Aprendiz)
                        .ThenInclude(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                      .Include(c => c.TestGenApreFkNavigation.Aprendiz.EstadoAprendiz)
                      .Include(c => c.TestGenApreFkNavigation.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.TestGenPsicoFkNavigation)
                        );


            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            var resultados = datos.Select(c => new
            {
                c.TestGenCodigo,
                aprendiz = MapearAprendizFicha(c.TestGenApreFkNavigation),
                psicologo = c.TestGenPsicoFkNavigation,
                FechaRealizacion = c.TestGenFechaRealiz,
                Resultados = c.TestGenResultados,
                Recomendaciones = c.TestGenRecomendacion
            });

            return Ok(resultados);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var datos = await _uow.TestGeneral.ObtenerTodoConCondicion(e => e.TestGenEstado == "activo" && e.TestGenCodigo == id,
                e => e.Include(c => c.TestGenApreFkNavigation)
                    .ThenInclude(c => c.Aprendiz)
                        .ThenInclude(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                      .Include(c => c.TestGenApreFkNavigation.Aprendiz.EstadoAprendiz)
                      .Include(c => c.TestGenApreFkNavigation.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.TestGenPsicoFkNavigation)
                        );


            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            var resultados = datos.Select(c => new
            {
                c.TestGenCodigo,
                aprendiz = MapearAprendizFicha(c.TestGenApreFkNavigation),
                psicologo = c.TestGenPsicoFkNavigation,
                FechaRealizacion = c.TestGenFechaRealiz,
                Resultados = c.TestGenResultados,
                Recomendaciones = c.TestGenRecomendacion
            });

            return Ok(resultados);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] FiltroTestGeneralDTO f)
        {
            IQueryable<TestGeneral> q = _uow.TestGeneral.Query()
                .Include(c => c.TestGenApreFkNavigation)
                    .ThenInclude(c => c.Aprendiz)
                        .ThenInclude(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                      .Include(c => c.TestGenApreFkNavigation.Aprendiz.EstadoAprendiz)
                      .Include(c => c.TestGenApreFkNavigation.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.TestGenPsicoFkNavigation);

            if (f.Codigo.HasValue)
                q = q.Where(x => x.TestGenCodigo == f.Codigo.Value);

            if (!string.IsNullOrEmpty(f.TipoDocumento))
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.AprTipoDocumento == f.TipoDocumento);

            if (!string.IsNullOrEmpty(f.NroDocumento))
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.AprNroDocumento == f.NroDocumento);

            if (!string.IsNullOrEmpty(f.PrimerNombre))
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.AprNombre.ToLower().Contains(f.PrimerNombre.ToLower()));

            if (!string.IsNullOrEmpty(f.PrimerApellido))
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.AprApellido.ToLower().Contains(f.PrimerApellido.ToLower()));

            if (!string.IsNullOrEmpty(f.MunicipioNombre))
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.Municipio.CiuNombre.ToLower() == f.MunicipioNombre.ToLower());

            if (!string.IsNullOrEmpty(f.DepartamentoNombre))
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.Municipio.Regional.RegNombre.ToLower() == f.DepartamentoNombre.ToLower());

            if (f.EstadoAprendizID.HasValue)
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.EstadoAprendiz.EstAprCodigo == f.EstadoAprendizID.Value);

            if (!string.IsNullOrEmpty(f.Eps))
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.AprEps.ToLower() == f.Eps.ToLower());

            if (!string.IsNullOrEmpty(f.TipoPoblacion))
                q = q.Where(x => x.TestGenApreFkNavigation.Aprendiz.AprTipoPoblacion.ToLower() == f.TipoPoblacion.ToLower());

            if (!string.IsNullOrEmpty(f.PsicologoDocumento))
                q = q.Where(x => x.TestGenPsicoFkNavigation.PsiDocumento == f.PsicologoDocumento);

            if (f.FechaRealizacionDesde.HasValue)
            {
                var desde = f.FechaRealizacionDesde.Value.Date;
                q = q.Where(x => x.TestGenFechaRealiz.HasValue && x.TestGenFechaRealiz.Value.Date >= desde);
            }

            if (f.FechaRealizacionHasta.HasValue)
            {
                var hasta = f.FechaRealizacionHasta.Value.Date;
                q = q.Where(x => x.TestGenFechaRealiz.HasValue && x.TestGenFechaRealiz.Value.Date <= hasta);
            }




            var datos = await q.ToListAsync();

            if (!datos.Any())
                return NotFound("No se encontraron resultados con esos filtros.");

            var resultados = datos.Select(MapearTestGeneral);

            return Ok(resultados);
        }



        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPost]
        public async Task<IActionResult> CrearTestGeneral([FromBody] TestGeneralDTO nuevoRegistro)
        {

            if (nuevoRegistro == null)
            {
                return BadRequest("El cuerpo no puede ser nulo.");
            }

            var testNew = new TestGeneral
            {
                TestGenApreFk = nuevoRegistro.TestGenApreFk,
                TestGenPsicoFk = nuevoRegistro.TestGenPsicoFk,
                TestGenFechaRealiz = nuevoRegistro.TestGenFechaRealiz,
                TestGenResultados = nuevoRegistro.TestGenResultados,
                TestGenRecomendacion= nuevoRegistro.TestGenRecomendacion
            };
            await _uow.TestGeneral.Agregar(testNew);
            await _uow.SaveChangesAsync();



            var datos = await _uow.TestGeneral.ObtenerTodoConCondicion(e => e.TestGenEstado == "activo" && e.TestGenCodigo == testNew.TestGenCodigo,
                e => e.Include(c => c.TestGenApreFkNavigation)
                        .ThenInclude(c => c.Aprendiz)
                            .ThenInclude(c => c.Municipio)
                                .ThenInclude(c => c.Regional)
                        .Include(c => c.TestGenApreFkNavigation.Aprendiz.EstadoAprendiz)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Centro)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.TestGenPsicoFkNavigation)
                );

            if (datos == null || !datos.Any())
            {
                return NotFound("No se encontró este elemento");
            }

            return Ok(new
            {
                mensaje = "Programa creado correctamente!",
                datos
            });
        }

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("editar/{id}")]
        public async Task<IActionResult> EditarTest(int id, [FromBody] TestGeneralDTO nuevoRegistro)
        {
            if (id.ToString() == "")
            {
                return BadRequest("El ID no debe ser nulo");
            }
            var testEncontrado = await _uow.TestGeneral.ObtenerPorID(id);
            
            if (testEncontrado == null || testEncontrado.TestGenEstado == "inactivo")
            {
                return NotFound("No se encontró este ID");
            }

            testEncontrado.TestGenApreFk = nuevoRegistro.TestGenApreFk;
            testEncontrado.TestGenPsicoFk = nuevoRegistro.TestGenPsicoFk;
            testEncontrado.TestGenFechaRealiz = nuevoRegistro.TestGenFechaRealiz;
            testEncontrado.TestGenResultados = nuevoRegistro.TestGenResultados;
            testEncontrado.TestGenRecomendacion = nuevoRegistro.TestGenRecomendacion;


            _uow.TestGeneral.Actualizar(testEncontrado);
            await _uow.SaveChangesAsync();

            var datos = await _uow.TestGeneral.ObtenerTodoConCondicion(e => e.TestGenEstado == "activo" && e.TestGenCodigo == testEncontrado.TestGenCodigo,
                e => e.Include(c => c.TestGenApreFkNavigation)
                        .ThenInclude(c => c.Aprendiz)
                            .ThenInclude(c => c.Municipio)
                                .ThenInclude(c => c.Regional)
                        .Include(c => c.TestGenApreFkNavigation.Aprendiz.EstadoAprendiz)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Centro)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.TestGenApreFkNavigation.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                        .Include(c => c.TestGenPsicoFkNavigation)
                );

            if (datos == null || !datos.Any())
            {
                return NotFound("No se encontró este elemento");
            }

            return Ok(new
            {
                mensaje = "Programa editado correctamente!",
                datos
            });
        }



        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPut("eliminar/{id}")]
        public async Task<IActionResult> EliminarTest(int id)
        {
            var testEncontrado = await _uow.TestGeneral.ObtenerPorID(id);
            if (testEncontrado.TestGenEstado == "inactivo" || testEncontrado == null)
            {
                return NotFound("No se encontró este ID");
            }
            testEncontrado.TestGenEstado = "inactivo";

            _uow.TestGeneral.Actualizar(testEncontrado);
            await _uow.SaveChangesAsync();

            return Ok("Se ha eliminado correctamente!");
        }

        /* ─────────── Helpers de autenticación ─────────── */

        private bool TryObtenerPsicologoIdAutenticado(out int psicologoId)
        {
            psicologoId = 0;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("nameid");
            return !string.IsNullOrWhiteSpace(userId) && int.TryParse(userId, out psicologoId);
        }

        private bool TryObtenerAprendizIdAutenticado(out int aprendizId)
        {
            aprendizId = 0;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("nameid");
            return !string.IsNullOrWhiteSpace(userId) && int.TryParse(userId, out aprendizId);
        }

        /* ─────────── Asignar test a un aprendiz ─────────── */

        /// <summary>Psicólogo asigna una plantilla de test a un aprendiz.</summary>
        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPost("asignar")]
        public async Task<IActionResult> AsignarTest([FromBody] AsignarTestDTO dto)
        {
            if (!TryObtenerPsicologoIdAutenticado(out var psicologoId))
                return Forbid();

            var plantilla = await _uow.PlantillaTest.ObtenerPrimero(p => p.PlaTstCodigo == dto.PlantillaId && p.PlaTstEstadoRegistro == "activo");
            if (plantilla == null)
                return NotFound(new { message = "Plantilla no encontrada." });

            var aprFicha = await _uow.AprendizFicha.ObtenerPrimero(a => a.AprFicCodigo == dto.AprendizFichaId);
            if (aprFicha == null)
                return NotFound(new { message = "Aprendiz-ficha no encontrado." });

            var test = new TestGeneral
            {
                TestGenApreFk = dto.AprendizFichaId,
                TestGenPsicoFk = psicologoId,
                TestGenPlantillaFk = dto.PlantillaId,
                TestGenEstadoTest = "asignado",
                TestGenFechaRealiz = null
            };

            await _uow.TestGeneral.Agregar(test);
            await _uow.SaveChangesAsync();

            return Ok(new { message = "Test asignado correctamente.", testId = test.TestGenCodigo });
        }

        /* ─────────── Tests de un aprendiz (vista psicólogo) ─────────── */

        /// <summary>Lista los tests asignados a un aprendiz (vista psicólogo).</summary>
        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet("por-aprendiz/{aprendizFichaId}")]
        public async Task<IActionResult> TestsPorAprendiz(int aprendizFichaId)
        {
            var datos = await _uow.TestGeneral.Query()
                .Where(t => t.TestGenEstado == "activo" && t.TestGenApreFk == aprendizFichaId && t.TestGenPlantillaFk != null)
                .Include(t => t.TestGenPlantillaFkNavigation)
                .Include(t => t.TestRespuestas.Where(r => r.TesResEstadoRegistro == "activo"))
                    .ThenInclude(r => r.TesResPreguntaFkNavigation)
                .Include(t => t.TestRespuestas.Where(r => r.TesResEstadoRegistro == "activo"))
                    .ThenInclude(r => r.TesResOpcionFkNavigation)
                .OrderByDescending(t => t.TestGenFechaRealiz ?? DateTime.MinValue)
                .ToListAsync();

            var resultados = datos.Select(t => new
            {
                t.TestGenCodigo,
                PlantillaNombre = t.TestGenPlantillaFkNavigation?.PlaTstNombre,
                PlantillaDescripcion = t.TestGenPlantillaFkNavigation?.PlaTstDescripcion,
                t.TestGenEstadoTest,
                FechaRealizacion = t.TestGenFechaRealiz,
                Resultados = t.TestGenResultados,
                Recomendaciones = t.TestGenRecomendacion,
                Respuestas = t.TestRespuestas.Select(r => new
                {
                    PreguntaId = r.TesResPreguntaFk,
                    PreguntaTexto = r.TesResPreguntaFkNavigation?.PlaPrgTexto,
                    OpcionId = r.TesResOpcionFk,
                    OpcionTexto = r.TesResOpcionFkNavigation?.PlaOpcTexto,
                    r.TesResFechaRespuesta
                })
            });

            return Ok(resultados);
        }

        /* ─────────── Endpoints para el aprendiz ─────────── */

        /// <summary>Tests asignados al aprendiz autenticado.</summary>
        [Authorize(Roles = Roles.Aprendiz)]
        [HttpGet("mis-tests")]
        public async Task<IActionResult> MisTests()
        {
            if (!TryObtenerAprendizIdAutenticado(out var aprendizId))
                return Forbid();

            var fichas = await _uow.AprendizFicha.Query()
                .Where(af => af.AprFicAprendizFk == aprendizId)
                .Select(af => af.AprFicCodigo)
                .ToListAsync();

            if (!fichas.Any())
                return Ok(Array.Empty<object>());

            var datos = await _uow.TestGeneral.Query()
                .Where(t => t.TestGenEstado == "activo"
                    && t.TestGenPlantillaFk != null
                    && fichas.Contains(t.TestGenApreFk ?? 0))
                .Include(t => t.TestGenPlantillaFkNavigation)
                .Include(t => t.TestGenPsicoFkNavigation)
                .OrderByDescending(t => t.TestGenCodigo)
                .ToListAsync();

            return Ok(datos.Select(t => new
            {
                t.TestGenCodigo,
                PlantillaNombre = t.TestGenPlantillaFkNavigation?.PlaTstNombre,
                PlantillaDescripcion = t.TestGenPlantillaFkNavigation?.PlaTstDescripcion,
                t.TestGenEstadoTest,
                FechaRealizacion = t.TestGenFechaRealiz,
                Psicologo = t.TestGenPsicoFkNavigation == null ? null : new
                {
                    t.TestGenPsicoFkNavigation.PsiCodigo,
                    t.TestGenPsicoFkNavigation.PsiNombre,
                    t.TestGenPsicoFkNavigation.PsiApellido
                }
            }));
        }

        /// <summary>Preguntas y opciones del test para que el aprendiz responda.</summary>
        [Authorize(Roles = Roles.Aprendiz)]
        [HttpGet("mis-tests/{id}/preguntas")]
        public async Task<IActionResult> PreguntasDelTest(int id)
        {
            if (!TryObtenerAprendizIdAutenticado(out var aprendizId))
                return Forbid();

            var fichas = await _uow.AprendizFicha.Query()
                .Where(af => af.AprFicAprendizFk == aprendizId)
                .Select(af => af.AprFicCodigo)
                .ToListAsync();

            var test = await _uow.TestGeneral.Query()
                .Where(t => t.TestGenCodigo == id && t.TestGenEstado == "activo" && fichas.Contains(t.TestGenApreFk ?? 0))
                .Include(t => t.TestGenPlantillaFkNavigation)
                    .ThenInclude(p => p!.Preguntas.Where(q => q.PlaPrgEstadoRegistro == "activo"))
                        .ThenInclude(q => q.Opciones.Where(o => o.PlaOpcEstadoRegistro == "activo"))
                .FirstOrDefaultAsync();

            if (test == null)
                return NotFound(new { message = "Test no encontrado." });

            var plantilla = test.TestGenPlantillaFkNavigation;
            if (plantilla == null)
                return NotFound(new { message = "Plantilla asociada no encontrada." });

            return Ok(new
            {
                test.TestGenCodigo,
                test.TestGenEstadoTest,
                PlantillaNombre = plantilla.PlaTstNombre,
                Preguntas = plantilla.Preguntas
                    .OrderBy(q => q.PlaPrgOrden)
                    .Select(q => new
                    {
                        q.PlaPrgCodigo,
                        q.PlaPrgTexto,
                        q.PlaPrgTipo,
                        q.PlaPrgOrden,
                        Opciones = q.Opciones.OrderBy(o => o.PlaOpcOrden).Select(o => new
                        {
                            o.PlaOpcCodigo,
                            o.PlaOpcTexto,
                            o.PlaOpcOrden
                        })
                    })
            });
        }

        /// <summary>El aprendiz envía sus respuestas.</summary>
        [Authorize(Roles = Roles.Aprendiz)]
        [HttpPost("mis-tests/{id}/responder")]
        public async Task<IActionResult> ResponderTest(int id, [FromBody] ResponderTestDTO dto)
        {
            if (!TryObtenerAprendizIdAutenticado(out var aprendizId))
                return Forbid();

            var fichas = await _uow.AprendizFicha.Query()
                .Where(af => af.AprFicAprendizFk == aprendizId)
                .Select(af => af.AprFicCodigo)
                .ToListAsync();

            var test = await _uow.TestGeneral.ObtenerPrimero(
                t => t.TestGenCodigo == id && t.TestGenEstado == "activo" && fichas.Contains(t.TestGenApreFk ?? 0));

            if (test == null)
                return NotFound(new { message = "Test no encontrado." });

            if (test.TestGenEstadoTest == "completado")
                return BadRequest(new { message = "Este test ya fue completado." });

            if (dto.Respuestas == null || !dto.Respuestas.Any())
                return BadRequest(new { message = "Debe enviar al menos una respuesta." });

            var ctx = _uow.ObtenerContexto();

            foreach (var item in dto.Respuestas)
            {
                await ctx.TestRespuestas.AddAsync(new TestRespuesta
                {
                    TesResTestFk = id,
                    TesResPreguntaFk = item.PreguntaId,
                    TesResOpcionFk = item.OpcionId,
                    TesResFechaRespuesta = DateTime.UtcNow
                });
            }

            test.TestGenEstadoTest = "completado";
            test.TestGenFechaRealiz = DateTime.UtcNow;
            _uow.TestGeneral.Actualizar(test);

            await _uow.SaveChangesAsync();

            return Ok(new { message = "Respuestas registradas correctamente." });
        }
        
    }
}
