using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Models.DTO.Filtros;
using API_healthyMind.Repositorios.Interfaces;
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
    public class AprendizFichaController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;
        


        public AprendizFichaController(IUnidadDeTrabajo uow)
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
                        d.Ficha.programaFormacion.Area,
                        d.Ficha.programaFormacion.Centro
                    }
                }

            };
        }


        // GET: NivelFormacionController
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.AprendizFicha.ObtenerTodoConCondicion(e => e.AprFicEstadoRegistro == "activo",
                e => e.Include(c => c.Aprendiz)
                        .ThenInclude(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                      .Include(c => c.Aprendiz.EstadoAprendiz)
                      .Include(c => c.Ficha)
                        .ThenInclude(c => c.programaFormacion)
                            .ThenInclude(c => c.Centro)
                        .Include(c => c.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.NivelFormacion)
                        .Include(c => c.Ficha)
                            .ThenInclude(c => c.programaFormacion)
                                .ThenInclude(c => c.Area)
                                    .ThenInclude(c => c.AreaPsicologo)

                        );
                            
                       

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            var resultados = datos.Select(c => MapearAprendizFicha(c));

            return Ok(resultados);
        }


        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] FiltroAprendizFichaDTO f)
        {
            IQueryable<AprendizFicha> q = _uow.AprendizFicha.Query()
                .Include(x => x.Aprendiz.EstadoAprendiz)
                .Include(x => x.Aprendiz)
                    .ThenInclude(a => a.Municipio)
                        .ThenInclude(m => m.Regional)
                .Include(x => x.Ficha)
                    .ThenInclude(fic => fic.programaFormacion)
                        .ThenInclude(p => p.Area)
                            .ThenInclude(a => a.AreaPsicologo)
                .Include(x => x.Ficha)
                    .ThenInclude(fic => fic.programaFormacion)
                        .ThenInclude(p => p.NivelFormacion)
                .Include(x => x.Ficha)
                    .ThenInclude(fic => fic.programaFormacion)
                        .ThenInclude(p => p.Centro)
                            .ThenInclude(c => c.Regional);

            // ======================
            //    FILTROS
            // ======================

            if (f.FichaCodigo.HasValue)
                q = q.Where(x => x.Ficha.FicCodigo == f.FichaCodigo.Value);

            if (!string.IsNullOrEmpty(f.AreaNombre))
                q = q.Where(x => x.Ficha.programaFormacion.Area.AreaNombre.ToLower()
                    .Contains(f.AreaNombre.ToLower()));

            if (!string.IsNullOrEmpty(f.ProgramaNombre))
                q = q.Where(x => x.Ficha.programaFormacion.ProgNombre.ToLower()
                    .Contains(f.ProgramaNombre.ToLower()));

            if (f.PsicologoID.HasValue)
                q = q.Where(x => x.Ficha.programaFormacion.Area.AreaPsicologo.PsiCodigo == f.PsicologoID.Value);

            if (!string.IsNullOrEmpty(f.TipoPoblacion))
                q = q.Where(x => x.Aprendiz.AprTipoPoblacion.ToLower() == f.TipoPoblacion.ToLower());

            if (!string.IsNullOrEmpty(f.Eps))
                q = q.Where(x => x.Aprendiz.AprEps.ToLower() == f.Eps.ToLower());

            if (f.EstadoAprendizID.HasValue)
                q = q.Where(x => x.Aprendiz.EstadoAprendiz.EstAprCodigo == f.EstadoAprendizID.Value);

            if (!string.IsNullOrEmpty(f.CentroNombre))
                q = q.Where(x => x.Ficha.programaFormacion.Centro.CenNombre.ToLower()
                    .Contains(f.CentroNombre.ToLower()));

            if (!string.IsNullOrEmpty(f.Jornada))
                q = q.Where(x => x.Ficha.FicJornada.ToLower() == f.Jornada.ToLower());

            q = q.Where(x => x.AprFicEstadoRegistro.ToLower() == "activo");


            // ======================
            //  EJECUCIÓN
            // ======================

            var datos = await q.ToListAsync();

            if (!datos.Any())
                return NotFound("No se encontraron resultados con esos filtros.");

            var resultado = datos.Select(MapearAprendizFicha);

            return Ok(resultado);
        }
        

        [HttpGet("estadistica/por-ficha")]
        public async Task<IActionResult> GetRegistrosPorMes()
        {
            var resultado = await _uow.AprendizFicha.Query()
                .GroupBy(x => x.AprFicFichaFk)
                .Select(g => new
                {
                    Ficha = g.Key,
                    Total_Aprendices = g.Count()
                })
                .OrderBy(x => x.Ficha)
                .ToListAsync();

            return Ok(resultado);
        }


        [HttpPost]
        public async Task<IActionResult> CrearRegistro(AprendizFichaDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("El cuerpo no debe estar vacio!");
            }

            var nuevoReg = new AprendizFicha
            {
                AprFicAprendizFk = dto.AprFicAprendizFk,
                AprFicFichaFk = dto.AprFicFichaFk
            };

            await _uow.AprendizFicha.Agregar(nuevoReg);
            await _uow.SaveChangesAsync();
            return Ok(new
            {
                mensaje = "Se ha creado el registro exitosamente",
                nuevoReg
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarInformacion(int id, [FromBody] AprendizFichaDTO dto)
        {
            var aprEncontrado = await _uow.AprendizFicha.ObtenerTodoConCondicion(a => a.Aprendiz.AprNroDocumento == id && a.AprFicEstadoRegistro == "activo");

            if (!aprEncontrado.Any())
            {
                return NotFound("No se encontró este aprendiz");
            }

            var resultado = aprEncontrado.FirstOrDefault();

            
            resultado.AprFicAprendizFk = dto.AprFicAprendizFk;
            resultado.AprFicFichaFk= dto.AprFicFichaFk;
            

            _uow.AprendizFicha.Actualizar(resultado);
            await _uow.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Se han editado correctamente los datos!",
                resultado
            });
        }


        [HttpPut("eliminar")]
        public async Task<IActionResult> EliminarAprendiz(int documento, [FromBody] RazonEliminacionDTO dto)
        {
            var aprEncontrado = await _uow.AprendizFicha.ObtenerTodoConCondicion(a => a.Aprendiz.AprNroDocumento == documento && a.AprFicEstadoRegistro == "activo");

            var user = aprEncontrado.FirstOrDefault();

            if (user == null)
                return NotFound("No se encontró este id.");

            user.AprFicEstadoRegistro = "inactivo";

            _uow.AprendizFicha.Actualizar(user);
            await _uow.SaveChangesAsync();
            return Ok("Se ha eliminado correctamente ");


        }

    }
}
