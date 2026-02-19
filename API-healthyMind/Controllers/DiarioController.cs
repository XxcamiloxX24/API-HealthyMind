using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Models.DTO.Filtros;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CualquierRol")]
    public class DiarioController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;
        


        public DiarioController(IUnidadDeTrabajo uow)
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

        private static object MapearAprendizDiario(Diario d)
        {
            return new
            {
                d.DiaCodigo,
                d.DiaTitulo,
                d.DiaFechaCreacion,
                Aprendiz = MapearAprendiz(d.aprendiz)
                

            };
        }


        // GET: NivelFormacionController
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.Diario.ObtenerTodoConCondicion(e => e.DiaEstadoRegistro == "activo" || e.DiaEstadoRegistro == "inactivo",
                e => e.Include(c => c.aprendiz)
                        .ThenInclude(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                      .Include(c => c.aprendiz.EstadoAprendiz)
                        );
                            
                       

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            var resultados = datos.Select(c => MapearAprendizDiario(c));

            return Ok(resultados);
        }

        [HttpGet("activos")]
        public async Task<IActionResult> ObtenerTodosActivos()
        {
            var datos = await _uow.Diario.ObtenerTodoConCondicion(e => e.DiaEstadoRegistro == "activo",
                e => e.Include(c => c.aprendiz)
                        .ThenInclude(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                      .Include(c => c.aprendiz.EstadoAprendiz)
                        );



            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            var resultados = datos.Select(c => MapearAprendizDiario(c));

            return Ok(resultados);
        }
        

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] FiltroDiarioDTO f)
        {
            IQueryable<Diario> q = _uow.Diario.Query()
                .Include(x => x.aprendiz.EstadoAprendiz)
                .Include(x => x.aprendiz)
                    .ThenInclude(a => a.Municipio)
                        .ThenInclude(m => m.Regional)
                .Include(x => x.aprendiz.AprendizFichas)
                    .ThenInclude(fic => fic.Ficha)
                        .ThenInclude(p => p.programaFormacion)
                            .ThenInclude(a => a.Area)
                                .ThenInclude(a => a.AreaPsicologo)
                .Include(x => x.aprendiz.AprendizFichas)
                    .ThenInclude(x => x.Ficha)
                        .ThenInclude(fic => fic.programaFormacion)
                            .ThenInclude(p => p.NivelFormacion)
                .Include(x => x.aprendiz.AprendizFichas)
                    .ThenInclude(c => c.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro);



            // ======================
            //    FILTROS
            // ======================

            if (f.FichaCodigo.HasValue)
                q = q.Where(x => x.aprendiz.AprendizFichas.Any(af => af.Ficha.FicCodigo == f.FichaCodigo.Value));

            if (!string.IsNullOrEmpty(f.AreaNombre))
                q = q.Where(x =>
                    x.aprendiz.AprendizFichas.Any(af =>
                        af.Ficha.programaFormacion.Area.AreaNombre
                            .ToLower()
                            .Contains(f.AreaNombre.ToLower())
                ));


            if (!string.IsNullOrEmpty(f.ProgramaNombre))
                q = q.Where(x =>
                    x.aprendiz.AprendizFichas.Any(af =>
                        af.Ficha.programaFormacion.ProgNombre
                            .ToLower()
                            .Contains(f.ProgramaNombre.ToLower())
                ));


            if (!string.IsNullOrEmpty(f.AprendizId))
                q = q.Where(x => x.aprendiz.AprNroDocumento == f.AprendizId);


            if (!string.IsNullOrEmpty(f.TipoPoblacion))
                q = q.Where(x => x.aprendiz.AprTipoPoblacion.ToLower() == f.TipoPoblacion.ToLower());

            if (!string.IsNullOrEmpty(f.Eps))
                q = q.Where(x => x.aprendiz.AprEps.ToLower() == f.Eps.ToLower());

            if (f.EstadoAprendizID.HasValue)
                q = q.Where(x => x.aprendiz.EstadoAprendiz.EstAprCodigo == f.EstadoAprendizID.Value);

            if (!string.IsNullOrEmpty(f.CentroNombre))
                q = q.Where(x =>
                    x.aprendiz.AprendizFichas.Any(af =>
                        af.Ficha.programaFormacion.Centro.CenNombre
                            .ToLower()
                            .Contains(f.CentroNombre.ToLower())
                ));


            if (!string.IsNullOrEmpty(f.Jornada))
                q = q.Where(x =>
                    x.aprendiz.AprendizFichas.Any(af =>
                        af.Ficha.FicJornada.ToLower() == f.Jornada.ToLower()
                ));


            q = q.Where(x => x.DiaEstadoRegistro.ToLower() == "activo" && x.aprendiz.AprEstadoRegistro.ToLower() == "activo");


            // ======================
            //  EJECUCIÓN
            // ======================

            var datos = await q.ToListAsync();

            if (!datos.Any())
                return NotFound("No se encontraron resultados con esos filtros.");

            var resultado = datos.Select(MapearAprendizDiario);

            return Ok(resultado);
        }
        
        
        [HttpPost]
        public async Task<IActionResult> CrearRegistro(DiarioDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("El cuerpo no debe estar vacio!");
            }

            var nuevoReg = new Diario
            {
                DiaTitulo = dto.DiaTitulo,
                DiaFechaCreacion = DateOnly.FromDateTime(DateTime.Now),
                DiaAprendizFk = dto.DiaAprendizFk,
            };

            await _uow.Diario.Agregar(nuevoReg);
            await _uow.SaveChangesAsync();

            var datos = (await _uow.Diario.ObtenerTodoConCondicion(e => e.DiaEstadoRegistro == "activo" && e.DiaCodigo == nuevoReg.DiaCodigo,
                e => e.Include(c => c.aprendiz)
                        .ThenInclude(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                      .Include(c => c.aprendiz.EstadoAprendiz)
                        )).FirstOrDefault();

            var resultado = MapearAprendizDiario(datos);

            return Ok(new
            {
                mensaje = "Se ha creado el registro exitosamente",
                resultado
            });
        }

        [HttpPut("editar/{id}")]
        public async Task<IActionResult> EditarInformacion(int id, [FromBody] DiarioDTO dto)
        {
            var diarioEncontrado = await _uow.Diario.ObtenerTodoConCondicion(a => a.DiaCodigo == id && a.DiaEstadoRegistro == "activo");

            if (!diarioEncontrado.Any())
            {
                return NotFound("No se encontró este aprendiz");
            }

            var resultado = diarioEncontrado.FirstOrDefault();

            
            resultado.DiaTitulo = dto.DiaTitulo;
            resultado.DiaAprendizFk = dto.DiaAprendizFk;
            

            _uow.Diario.Actualizar(resultado);
            await _uow.SaveChangesAsync();

            var datos = (await _uow.Diario.ObtenerTodoConCondicion(e => e.DiaEstadoRegistro == "activo" 
            && e.DiaCodigo == resultado.DiaCodigo,
                e => e.Include(c => c.aprendiz)
                        .ThenInclude(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                      .Include(c => c.aprendiz.EstadoAprendiz)
                        )).FirstOrDefault();

            var resultadoEditado = MapearAprendizDiario(datos);

            return Ok(new
            {
                mensaje = "Se han editado correctamente los datos!",
                resultadoEditado
            });
        }


        /// <summary>
        /// Soft delete del diario (cambia estado a inactivo). Aprendiz: solo su propio diario. Admin/Psicólogo: cualquiera.
        /// </summary>
        [HttpPut("eliminar/{id}")]
        public async Task<IActionResult> EliminarDiario(int id)
        {
            var diarioEncontrado = await _uow.Diario.ObtenerTodoConCondicion(a => a.DiaCodigo == id 
            && a.DiaEstadoRegistro == "activo",
                a => a.Include(d => d.aprendiz));

            var diario = diarioEncontrado.FirstOrDefault();

            if (diario == null)
                return NotFound("No se encontró este id.");

            // Si es Aprendiz, solo puede eliminar (inactivar) su propio diario
            if (User.IsInRole(Models.Roles.Aprendiz))
            {
                var miId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("nameid");
                if (string.IsNullOrEmpty(miId) || diario.DiaAprendizFk.ToString() != miId)
                    return StatusCode(403, "Solo puedes eliminar tu propio diario.");
            }

            diario.DiaEstadoRegistro = "inactivo";

            _uow.Diario.Actualizar(diario);
            await _uow.SaveChangesAsync();
            return Ok("Se ha eliminado correctamente ");
        }

    }
}
