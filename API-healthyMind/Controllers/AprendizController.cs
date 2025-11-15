using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
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
    public class AprendizController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public AprendizController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }

        private static object MapearAprendiz(Aprendiz d)
        {
            return new
            {
                Codigo = d.AprCodigo,
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

        private static object MapearAprendizInactivo(Aprendiz d)
        {
            return new
            {
                Codigo = d.AprCodigo,
                d.AprFechaEliminacion,
                d.AprRazonEliminacion,
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
        // GET: NivelFormacionController
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.Aprendiz.ObtenerTodoConCondicion(e => e.AprEstadoRegistro == "activo",
                e => e.Include(c => c.Municipio)
                        .ThenInclude(c => c.Regional)
                      .Include(c => c.EstadoAprendiz));

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró ningun registro!");
            }

            var resultado = datos.Select(MapearAprendiz);

            return Ok(resultado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var datos = await _uow.Aprendiz.ObtenerTodoConCondicion(e => e.AprEstadoRegistro == "activo" && e.AprNroDocumento == id,
                e => e.Include(c => c.Municipio)
                        .ThenInclude(c => c.Regional)
                      .Include(c => c.EstadoAprendiz));

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró el registro!");
            }
            var resultado = datos.Select(MapearAprendiz);

            return Ok(resultado);
        }

        [HttpGet("inactivos")]
        public async Task<IActionResult> ObtenerTodosEliminados()
        {
            var datos = await _uow.Aprendiz.ObtenerTodoConCondicion(e => e.AprEstadoRegistro == "inactivo",
                e => e.Include(c => c.Municipio)
                        .ThenInclude(c => c.Regional)
                      .Include(c => c.EstadoAprendiz));

            if (!datos.Any() || datos == null)
            {
                return Ok("No se encontró ningun registro!");
            }

            var resultado = datos.Select(MapearAprendizInactivo);

            return Ok(resultado);
        }

        [HttpGet("inactivo/{id}")]
        public async Task<IActionResult> ObtenerPorIdInactivo(int id)
        {
            var datos = await _uow.Aprendiz.ObtenerTodoConCondicion(e => e.AprEstadoRegistro == "inactivo" && e.AprNroDocumento == id,
                e => e.Include(c => c.Municipio)
                        .ThenInclude(c => c.Regional)
                      .Include(c => c.EstadoAprendiz));

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró el registro!");
            }
            var resultado = datos.Select(d => new
            {
                Codigo = d.AprCodigo,
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
            });

            return Ok(resultado);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] FiltroAprendizDTO f)
        {
            IQueryable<Aprendiz> q = _uow.Aprendiz.Query().Include(x => x.EstadoAprendiz).Include(x => x.Municipio).ThenInclude(m => m.Regional);

            if (f.Codigo.HasValue)
                q = q.Where(x => x.AprCodigo == f.Codigo.Value);

            if (!string.IsNullOrEmpty(f.TipoDocumento))
                q = q.Where(x => x.AprTipoDocumento == f.TipoDocumento);

            if (f.NroDocumento.HasValue)
                q = q.Where(x => x.AprNroDocumento == f.NroDocumento.Value);

            if (!string.IsNullOrEmpty(f.PrimerNombre))
                q = q.Where(x => x.AprNombre.ToLower().Contains(f.PrimerNombre.ToLower()));

            if (!string.IsNullOrEmpty(f.PrimerApellido))
                q = q.Where(x => x.AprApellido.ToLower().Contains(f.PrimerApellido.ToLower()));

            if (f.MunicipioID.HasValue)
                q = q.Where(x => x.Municipio.CiuCodigo == f.MunicipioID.Value);

            if (f.DepartamentoID.HasValue)
                q = q.Where(x => x.Municipio.Regional.RegCodigo == f.DepartamentoID.Value);

            if (f.EstadoAprendizID.HasValue)
                q = q.Where(x => x.EstadoAprendiz.EstAprCodigo == f.EstadoAprendizID.Value);

            if (!string.IsNullOrEmpty(f.Eps))
                q = q.Where(x => x.AprEps.ToLower() == f.Eps.ToLower());

            if (!string.IsNullOrEmpty(f.TipoPoblacion))
                q = q.Where(x => x.AprTipoPoblacion.ToLower() == f.TipoPoblacion.ToLower());

            var datos = await q.ToListAsync();

            if (!datos.Any())
                return NotFound("No se encontraron resultados con esos filtros.");

            var resultados = datos.Select(MapearAprendiz);

            return Ok(resultados);
        }


    }
}
