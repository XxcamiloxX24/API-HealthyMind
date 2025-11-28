using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Repositorios.Interfaces;
using API_healthyMind.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static API_healthyMind.Controllers.AprendizController;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PsicologoController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;
        private readonly IEmailService _emailService;


        public PsicologoController(IUnidadDeTrabajo uow, IEmailService emailService)
        {
            _uow = uow;
            _emailService = emailService;
        }
        // GET: NivelFormacionController
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.Psicologo.ObtenerTodoConCondicion(e => e.PsiEstadoRegistro == "activo");

            
            return Ok(datos);
        }

        [HttpGet("listar")]
        public async Task<IActionResult> ListarPsicologos([FromQuery] PaginacionDTO p)
        {
            if (p.TamanoPagina > 100) 
                p.TamanoPagina = 100;

            var query = _uow.Psicologo.Query();

            var totalRegistros = await query.CountAsync();

            var datos = await query
                .Skip((p.Pagina - 1) * p.TamanoPagina)
                .Take(p.TamanoPagina)
                .ToListAsync();

            return Ok(new
            {
                paginaActual = p.Pagina,
                tamanoPagina = p.TamanoPagina,
                totalRegistros,
                totalPaginas = (int)Math.Ceiling(totalRegistros / (double)p.TamanoPagina),
                datos
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorDocumento(string documento)
        {
            var datos = await _uow.Psicologo.ObtenerTodoConCondicion(e => e.PsiDocumento == documento && e.PsiEstadoRegistro == "activo");

            if (!datos.Any())
            {
                return NotFound("No se encontró este documento");
            }

            return Ok(datos);
        }

        [HttpGet("especialidad/{especialidad}")]
        public async Task<IActionResult> ObtenerPorNombre(string especialidad)
        {
            var datos = await _uow.Psicologo.ObtenerTodoConCondicion(e => e.PsiEspecialidad.ToLower() == especialidad.ToLower() && e.PsiEstadoRegistro == "activo");

            if (!datos.Any())
            {
                return NotFound("No se encontró ningun registro");
            }

            return Ok(datos);
        }

        [HttpGet("area/{areaNombre}")]
        public async Task<IActionResult> ObtenerPorArea(string areaNombre)
        {
            var datos = await _uow.Psicologo.Query()
                .Where(p =>
                    p.PsiEstadoRegistro == "activo" &&
                    p.Area.Any(a =>
                        a.AreaEstadoRegistro == "activo" &&
                        a.AreaNombre == areaNombre
                    )
                )
                .ToListAsync();

            if (!datos.Any())
                return NotFound("No se encontró ningún registro");

            return Ok(datos);
        }

        [HttpGet("estadistica/total-activos")]
        public async Task<IActionResult> GetTotalPsicologosActivos()
        {
            var total = await _uow.Psicologo
                .Query()
                .Where(a => a.PsiEstadoRegistro == "activo")
                .CountAsync();

            return Ok(new
            {
                totalPsicologos = total
            });
        }

        [HttpPost]
        public async Task<IActionResult> CrearArea([FromBody] PsicologoDTO nuevoPsicologo)
        {

            if (nuevoPsicologo == null)
            {
                return BadRequest("El cuerpo no puede ser nulo.");
            }

            var psicNew = new Psicologo
            {
                PsiDocumento = nuevoPsicologo.PsiDocumento,
                PsiNombre = nuevoPsicologo.PsiNombre,
                PsiApellido = nuevoPsicologo.PsiApellido,
                PsiEspecialidad = nuevoPsicologo.PsiEspecialidad,
                PsiTelefono = nuevoPsicologo.PsiTelefono,
                PsiFechaRegistro = DateTime.Now,
                PsiFechaNac = nuevoPsicologo.PsiFechaNac,
                PsiDireccion = nuevoPsicologo.PsiDireccion,
                PsiCorreoInstitucional = nuevoPsicologo.PsiCorreoInstitucional,
                PsiCorreoPersonal = nuevoPsicologo.PsiCorreoPersonal,
                PsiPassword = BCrypt.Net.BCrypt.HashPassword(nuevoPsicologo.PsiPassword),
                PsiFirma = nuevoPsicologo.PsiFirma
            };
            await _uow.Psicologo.Agregar(psicNew);
            await _uow.SaveChangesAsync();

            

            var datos = await _uow.Psicologo.ObtenerTodoConCondicion(
                e => e.PsiDocumento == psicNew.PsiDocumento && e.PsiEstadoRegistro == "activo");

            return Ok(new
            {
                mensaje = "Registro creado correctamente.",
                datos
            });
        }

        [HttpPut("editar/{documento}")]
        public async Task<IActionResult> EditarArea(int documento, [FromBody] PsicologoDTO psicRecibido)
        {
            if (documento.ToString() == "")
            {
                return BadRequest("El ID no debe ser nulo");
            }
            var psicEncontrado = await _uow.Psicologo.ObtenerPorID(documento);
            
            if (psicEncontrado == null)
            {
                return NotFound("No se encontró este ID");
            }

            psicEncontrado.PsiDocumento = psicRecibido.PsiDocumento;
            psicEncontrado.PsiNombre = psicRecibido.PsiNombre;
            psicEncontrado.PsiApellido = psicRecibido.PsiApellido;
            psicEncontrado.PsiEspecialidad = psicRecibido.PsiEspecialidad;
            psicEncontrado.PsiTelefono = psicRecibido.PsiTelefono;
            psicEncontrado.PsiFechaNac = psicRecibido.PsiFechaNac;
            psicEncontrado.PsiDireccion = psicRecibido.PsiDireccion;
            psicEncontrado.PsiCorreoInstitucional = psicRecibido.PsiCorreoInstitucional;
            psicEncontrado.PsiCorreoPersonal = psicRecibido.PsiCorreoPersonal;
            psicEncontrado.PsiFirma = psicRecibido.PsiFirma;



            _uow.Psicologo.Actualizar(psicEncontrado);
            await _uow.SaveChangesAsync();

            var datos = await _uow.Psicologo.ObtenerTodoConCondicion(
                e => e.PsiDocumento == psicEncontrado.PsiDocumento && e.PsiEstadoRegistro == "activo");

            return Ok(new
            {
                mensaje = "Psicologo editado correctamente.",
                datos
            });
        }

        public class CambiarPasswordPsicologoDTO
        {
            public string UsuarioDocumento { get; set; }
            public string PasswordActual { get; set; }
            public string PasswordNueva { get; set; }
        }

        [HttpPut("cambiar-password")]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordPsicologoDTO dto)
        {
            var psicologo = await _uow.Psicologo.ObtenerTodoConCondicion(c => c.PsiDocumento == dto.UsuarioDocumento);
            var resultado = psicologo.FirstOrDefault();

            if (resultado == null)
                return NotFound("El psicólogo no existe");

            // Verificar contraseña actual con BCrypt
            bool esCorrecta = BCrypt.Net.BCrypt.Verify(dto.PasswordActual, resultado.PsiPassword);

            if (!esCorrecta)
                return BadRequest("La contraseña actual no es correcta");

            // Guardar nueva contraseña encriptada
            resultado.PsiPassword = BCrypt.Net.BCrypt.HashPassword(dto.PasswordNueva);

            _uow.Psicologo.Actualizar(resultado);
            await _uow.SaveChangesAsync();

            return Ok("Contraseña actualizada correctamente");
        }


        [HttpPost("recuperar-password")]
        public async Task<IActionResult> RecuperarPassword([FromBody] SolicitarRecuperacionDTO dto)
        {
            var psico = await _uow.Psicologo.ObtenerTodoConCondicion(a =>
                a.PsiCorreoInstitucional == dto.Correo ||
                a.PsiCorreoPersonal == dto.Correo
            );

            var usuario = psico.FirstOrDefault();

            if (usuario == null)
                return NotFound("No existe un psicologo con ese correo.");

            // Crear JWT temporal
            var token = JwtPasswordHelper.GenerarToken(usuario.PsiDocumento, "psicologo");

            var link = $"{token}";

            await _emailService.SendAsync(dto.Correo, "Recuperación de contraseña", $"Copia este Token: {link}");

            return Ok("Se envió un enlace de recuperación al correo.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetearPasswordDTO dto)
        {
            var datosToken = JwtPasswordHelper.ValidarToken(dto.Token);

            if (datosToken == null)
                return BadRequest("Token inválido o expirado.");

            // Obtener aprendiz por documento del token
            var psico = (await _uow.Psicologo.ObtenerTodoConCondicion(
                                a => a.PsiDocumento == datosToken.Documento)).FirstOrDefault();

            if (psico == null)
                return NotFound("El aprendiz no existe.");

            if (psico.PsiEstadoRegistro != "activo")
                return BadRequest("El Psicologo está inactivo, no es posible cambiar la contraseña.");

            // Actualizar contraseña
            psico.PsiPassword = BCrypt.Net.BCrypt.HashPassword(dto.NuevaPassword);

            _uow.Psicologo.Actualizar(psico);
            await _uow.SaveChangesAsync();

            return Ok("Contraseña actualizada correctamente.");
        }


        [HttpPut("eliminar/{id}")]
        public async Task<IActionResult> EliminarPsicologo(string id)
        {
            var psicEncontrado = await _uow.Psicologo.ObtenerTodoConCondicion(c => c.PsiDocumento == id);
            var datos = psicEncontrado.FirstOrDefault();
            if (datos.PsiEstadoRegistro == "inactivo" || datos == null)
            {
                return NotFound("No se encontró este ID");
            }
            datos.PsiEstadoRegistro = "inactivo";

            _uow.Psicologo.Actualizar(datos);
            await _uow.SaveChangesAsync();

            return Ok("Se ha eliminado correctamente!");
        }
        
    }
}
