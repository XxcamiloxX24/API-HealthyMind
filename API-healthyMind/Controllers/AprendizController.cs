using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Repositorios.Interfaces;
using API_healthyMind.Services;
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
        private readonly IEmailService _emailService;


        public AprendizController(IUnidadDeTrabajo uow, IEmailService emailService)
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
                FechaCreacion = d.AprFechaCreacion,
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

        [HttpGet("estadistica/por-mes")]
        public async Task<IActionResult> GetRegistrosPorMes()
        {
            var resultado = await _uow.Aprendiz.Query()
                .GroupBy(x => x.AprFechaCreacion.Month)
                .Select(g => new
                {
                    Mes = g.Key,
                    Total = g.Count()
                })
                .OrderBy(x => x.Mes)
                .ToListAsync();

            return Ok(resultado);
        }

        [HttpPost("registro-inicial")]
        public async Task<IActionResult> RegistroInicial([FromBody] RegistroInicialDTO dto)
        {
            if (dto == null) return BadRequest();

            var existe = await _uow.Aprendiz
                .ObtenerTodoConCondicion(a => a.AprNroDocumento == dto.AprNroDocumento && a.AprEstadoRegistro == "activo");

            if (existe.Any())
                return BadRequest("El documento ya está registrado");

            var aprendiz = new Aprendiz
            {
                AprTipoDocumento = dto.AprTipoDocumento,
                AprNroDocumento = dto.AprNroDocumento,
                AprCorreoPersonal = dto.CorreoPersonal,
                AprPassword = BCrypt.Net.BCrypt.HashPassword(dto.AprPassword),
                AprFechaCreacion = DateTime.Now,
                AprEstadoRegistro = "inactivo"
            };

            await _uow.Aprendiz.Agregar(aprendiz);
            await _uow.SaveChangesAsync();

            // Generamos código
            var code = new Random().Next(100000, 999999).ToString();

            var v = new VerificationCode
            {
                AprendizId = aprendiz.AprCodigo,
                Codigo = code,
                Expiration = DateTime.UtcNow.AddMinutes(10)
            };

            await _uow.VerificationCode.Agregar(v);
            await _uow.SaveChangesAsync();

            // Enviar correo
            await _emailService.SendAsync(dto.CorreoPersonal,
                "Código de verificación - Healthy Mind",
                $"Tu código de verificación es: {code}");

            return Ok("Código enviado al correo.");
        }

        [HttpPost("verificar-codigo")]
        public async Task<IActionResult> VerificarCodigo(VerificarCodigoDTO dto)
        {
            var registro = await _uow.VerificationCode
                .ObtenerTodoConCondicion(v => v.AprendizId == dto.AprendizId && v.Codigo == dto.Codigo && v.Expiration > DateTime.UtcNow);

            if (!registro.Any())
                return BadRequest("Código inválido o expirado");

            var aprendiz = await _uow.Aprendiz.ObtenerPorID(dto.AprendizId);
            
            aprendiz.AprEstadoRegistro = "activo";

            _uow.Aprendiz.Actualizar(aprendiz);
            await _uow.SaveChangesAsync();

            return Ok("Cuenta verificada correctamente.");
        }

        [HttpPost("reenviar-codigo")]
        public async Task<IActionResult> ReenviarCodigo([FromBody] ReenviarCodigoDTO dto)
        {
            // Verificar si existe aprendiz
            var aprendiz = await _uow.Aprendiz.ObtenerTodoConCondicion(a => a.AprNroDocumento == dto.AprNroDocumento);
            if (!aprendiz.Any())
                return NotFound("No existe un usuario con ese documento.");

            var user = aprendiz.First();

            // Eliminar códigos anteriores
            var anteriores = await _uow.VerificationCode.ObtenerTodoConCondicion(v => v.AprendizId == user.AprCodigo);
            foreach (var item in anteriores)
                _uow.VerificationCode.Eliminar(item);

            // Crear nuevo código
            var codigo = new Random().Next(100000, 999999).ToString();

            var nuevo = new VerificationCode
            {
                AprendizId = user.AprCodigo,
                Codigo = codigo,
                Expiration = DateTime.Now.AddMinutes(10)
            };

            await _uow.VerificationCode.Agregar(nuevo);
            await _uow.SaveChangesAsync();

            // Enviar correo
            await _emailService.SendAsync(
                user.AprCorreoPersonal,
                "Nuevo código de verificación - Healthy Mind",
                $"Tu nuevo código es: {codigo}"
            );

            return Ok("Se envió un nuevo código de verificación.");
        }

        [HttpPut("completar-informacion")]
        public async Task<IActionResult> CompletarInformacion(int documento, [FromBody] AprendizDTO nuevoAprendiz)
        {
            if (nuevoAprendiz == null || nuevoAprendiz.Equals(null))
            {
                return BadRequest("El cuerpo no puede ser nulo.");
            }

            var aprEncontrado = await _uow.Aprendiz.ObtenerTodoConCondicion(e => e.AprEstadoRegistro == "activo" && e.AprNroDocumento == documento,
                e => e.Include(c => c.Municipio)
                        .ThenInclude(c => c.Regional)
                      .Include(c => c.EstadoAprendiz));

            var resultado = aprEncontrado.FirstOrDefault();

            resultado.AprFechaCreacion = DateTime.Now;
            resultado.AprFechaNac = nuevoAprendiz.AprFechaNac;
            resultado.AprNombre = nuevoAprendiz.AprNombre;
            resultado.AprSegundoNombre = nuevoAprendiz.AprSegundoNombre;
            resultado.AprApellido = nuevoAprendiz.AprApellido;
            resultado.AprSegundoApellido = nuevoAprendiz.AprSegundoApellido;
            resultado.AprCiudadFk = nuevoAprendiz.AprCiudadFk;
            resultado.AprDireccion = nuevoAprendiz.AprDireccion;
            resultado.AprCorreoInstitucional = nuevoAprendiz.AprCorreoInstitucional;
            resultado.AprTelefono = nuevoAprendiz.AprTelefono;
            resultado.AprEps = nuevoAprendiz.AprEps;
            resultado.AprPatologia = nuevoAprendiz.AprPatologia;
            resultado.AprEstadoAprFk = nuevoAprendiz.AprEstadoAprFk;
            resultado.AprTipoPoblacion = nuevoAprendiz.AprTipoPoblacion;
            resultado.AprAcudNombre = nuevoAprendiz.AprAcudNombre;
            resultado.AprAcudApellido = nuevoAprendiz.AprAcudApellido;
            resultado.AprTelefonoAcudiente = nuevoAprendiz.AprTelefonoAcudiente;
            await _uow.Aprendiz.Agregar(resultado);
            await _uow.SaveChangesAsync();



            var datos = await _uow.Aprendiz.ObtenerTodoConCondicion(e => e.AprEstadoRegistro == "activo" && e.AprNroDocumento == resultado.AprNroDocumento,
                e => e.Include(c => c.Municipio)
                        .ThenInclude(c => c.Regional)
                      .Include(c => c.EstadoAprendiz));

            if (!datos.Any() || datos == null)
            {
                return NotFound("No se encontró el registro!");
            }
            var resultados = datos.Select(MapearAprendiz);

            return Ok(new
            {
                mensaje = "Programa creado correctamente!",
                resultados
            });
        }

        [HttpPut("cambiar-correo")]
        public async Task<IActionResult> cambiarCorreo(int documento, string correo)
        {

            var aprendiz = await _uow.Aprendiz.ObtenerTodoConCondicion(a => a.AprNroDocumento == documento && a.AprEstadoRegistro == "inactivo");

            var user = aprendiz.FirstOrDefault();

            if (user == null)
                return NotFound("No existe un usuario inactivo con ese documento.");

            user.AprCorreoPersonal = correo;

            _uow.Aprendiz.Actualizar(user);
            await _uow.SaveChangesAsync();

            return Ok("Se ha actualizado correctamente!");
        }

        [HttpPut("cambiar-password")]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordDTO dto)
        {
            var APR = await _uow.Aprendiz.ObtenerTodoConCondicion(c => c.AprNroDocumento == dto.UsuarioDocumento);
            var resultado = APR.FirstOrDefault();

            if (resultado == null)
                return NotFound("El psicólogo no existe");

            // Verificar contraseña actual con BCrypt
            bool esCorrecta = BCrypt.Net.BCrypt.Verify(dto.PasswordActual, resultado.AprPassword);

            if (!esCorrecta)
                return BadRequest("La contraseña actual no es correcta");

            // Guardar nueva contraseña encriptada
            resultado.AprPassword = BCrypt.Net.BCrypt.HashPassword(dto.PasswordNueva);

            _uow.Aprendiz.Actualizar(resultado);
            await _uow.SaveChangesAsync();

            return Ok("Contraseña actualizada correctamente");
        }

        [HttpPost("recuperar-password")]
        public async Task<IActionResult> RecuperarPassword([FromBody] SolicitarRecuperacionDTO dto)
        {
            var aprendiz = await _uow.Aprendiz.ObtenerTodoConCondicion(a =>
                a.AprCorreoInstitucional == dto.Correo ||
                a.AprCorreoPersonal == dto.Correo
            );

            var usuario = aprendiz.FirstOrDefault();

            if (usuario == null)
                return NotFound("No existe un aprendiz con ese correo.");

            // Crear JWT temporal
            var token = JwtPasswordHelper.GenerarToken((int)usuario.AprNroDocumento, "aprendiz");

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
            var aprendiz = (await _uow.Aprendiz.ObtenerTodoConCondicion(
                                a => a.AprNroDocumento == datosToken.Documento)).FirstOrDefault();

            if (aprendiz == null)
                return NotFound("El aprendiz no existe.");

            if (aprendiz.AprEstadoRegistro != "activo")
                return BadRequest("El aprendiz está inactivo, no es posible cambiar la contraseña.");

            // Actualizar contraseña
            aprendiz.AprPassword = BCrypt.Net.BCrypt.HashPassword(dto.NuevaPassword);

            _uow.Aprendiz.Actualizar(aprendiz);
            await _uow.SaveChangesAsync();

            return Ok("Contraseña actualizada correctamente.");
        }




        [HttpPut("editar-informacion/{documento}")]
        public async Task<IActionResult> EditarInformacion(int Documento, [FromBody] AprendizEditarDTO dto)
        {
            var aprEncontrado = await _uow.Aprendiz.ObtenerTodoConCondicion(a => a.AprNroDocumento == Documento && a.AprEstadoRegistro == "activo");

            if (!aprEncontrado.Any())
            {
                return NotFound("No se encontró este aprendiz");
            }

            var resultado = aprEncontrado.FirstOrDefault();

            if (dto.AprNroDocumento != resultado.AprNroDocumento)
            {
                bool existeDoc = await _uow.Aprendiz.Existe(a =>
                    a.AprNroDocumento == dto.AprNroDocumento &&
                    a.AprCodigo != resultado.AprCodigo &&
                    a.AprEstadoRegistro == "activo"
                );

                if (existeDoc)
                    return BadRequest("El número de documento ya se encuentra registrado en otro aprendiz.");
            }


            resultado.AprTipoDocumento = dto.AprTipoDocumento;
            resultado.AprNroDocumento = dto.AprNroDocumento;
            resultado.AprCorreoPersonal = dto.AprCorreoPersonal;
            resultado.AprFechaNac = dto.AprFechaNac;
            resultado.AprNombre = dto.AprNombre;
            resultado.AprSegundoNombre = dto.AprSegundoNombre;
            resultado.AprApellido = dto.AprApellido;
            resultado.AprSegundoApellido = dto.AprSegundoApellido;
            resultado.AprCiudadFk = dto.AprCiudadFk;
            resultado.AprDireccion = dto.AprDireccion;
            resultado.AprCorreoInstitucional = dto.AprCorreoInstitucional;
            resultado.AprTelefono = dto.AprTelefono;
            resultado.AprEps = dto.AprEps;
            resultado.AprPatologia = dto.AprPatologia;
            resultado.AprEstadoAprFk = dto.AprEstadoAprFk;
            resultado.AprTipoPoblacion = dto.AprTipoPoblacion;
            resultado.AprAcudNombre = dto.AprAcudNombre;
            resultado.AprAcudApellido = dto.AprAcudApellido;
            resultado.AprTelefonoAcudiente = dto.AprTelefonoAcudiente;

            _uow.Aprendiz.Actualizar(resultado);
            await _uow.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Se han editado correctamente los datos!",
                resultado
            });
        }


        [HttpPut("eliminar-usuario/{documento}")]
        public async Task<IActionResult> EliminarAprendiz(int documento, [FromBody] RazonEliminacionDTO dto)
        {
            var aprEncontrado = await _uow.Aprendiz.ObtenerTodoConCondicion(a => a.AprNroDocumento == documento && a.AprEstadoRegistro == "activo");

            var user = aprEncontrado.FirstOrDefault();

            if (user == null)
                return NotFound("No se encontró este id.");

            user.AprRazonEliminacion = dto.RazonEliminacion;
            user.AprFechaEliminacion = DateTime.Now;
            user.AprEstadoRegistro = "inactivo";

            _uow.Aprendiz.Actualizar(user);
            await _uow.SaveChangesAsync();
            return Ok("Se ha eliminado correctamente ");


        }

    }
}
