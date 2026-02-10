using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using API_healthyMind.Models.DTO.Filtros;
using API_healthyMind.Repositorios.Interfaces;
using API_healthyMind.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CualquierRol")]
    public class AprendizController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;
        private readonly IEmailService _emailService;


        public AprendizController(IUnidadDeTrabajo uow, IEmailService emailService)
        {
            _uow = uow;
            _emailService = emailService;
        }

        public class PaginacionDTO
        {
            public int Pagina { get; set; } = 1;
            public int TamanoPagina { get; set; } = 10;
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
                TipoPoblacion = d.AprTipoPoblacion,
                EstadoRegistro = d.AprEstadoRegistro
            };
        }
        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var datos = await _uow.Aprendiz.ObtenerTodoConCondicion(e => e.AprEstadoRegistro == "activo" || e.AprEstadoRegistro == "inactivo",
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


        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet("listar")]
        public async Task<IActionResult> ListarAprendices([FromQuery] PaginacionDTO p)
        {
            if (p.TamanoPagina > 100)
                p.TamanoPagina = 100;

            var query = _uow.Aprendiz.Query()
                        .Include(c => c.Municipio)
                            .ThenInclude(c => c.Regional)
                        .Include(c => c.EstadoAprendiz)
                        .OrderByDescending(a => a.AprCodigo);

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

            var resultado = datos.Select(MapearAprendiz);

            return Ok(new
            {
                paginaActual = p.Pagina,
                paginaAnterior = p.Pagina > 1 ? p.Pagina - 1 : (int?)null,
                paginaSiguiente = p.Pagina < totalPaginas ? p.Pagina + 1 : (int?)null,
                tamanoPagina = p.TamanoPagina,
                totalRegistros,
                totalPaginas,
                resultado
            });
        }




        /// <summary>
        /// Obtiene un aprendiz por ID. Admin/Psicólogo: cualquier aprendiz. Aprendiz: solo su propio perfil.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            // Si es Aprendiz, solo puede consultar su propio perfil
            if (User.IsInRole(Roles.Aprendiz))
            {
                // El JWT puede guardar el ID en "nameid" (serializado) o en NameIdentifier (ClaimTypes)
                var miId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("nameid");
                if (string.IsNullOrEmpty(miId) || id.ToString() != miId)
                    return StatusCode(403, "Solo puedes consultar tu propio perfil.");
            }

            var datos = await _uow.Aprendiz.ObtenerTodoConCondicion(e => e.AprCodigo == id,
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

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet("busqueda-dinamica")]
        public async Task<IActionResult> BuscarDinamico([FromQuery] string texto)
        {
            if (string.IsNullOrWhiteSpace(texto) || texto.Length < 3)
                return BadRequest("Debe escribir al menos 3 caracteres.");

            texto = texto.ToLower();

            var query = _uow.Aprendiz.Query()
                .Include(a => a.Municipio)
                    .ThenInclude(m => m.Regional)
                .Include(a => a.EstadoAprendiz)
                .Where(a =>
                    a.AprNroDocumento.ToLower().Contains(texto) ||
                    a.AprTipoDocumento.ToLower().Contains(texto) ||

                    a.AprNombre.ToLower().Contains(texto) ||
                    a.AprSegundoNombre.ToLower().Contains(texto) ||
                    a.AprApellido.ToLower().Contains(texto) ||
                    a.AprSegundoApellido.ToLower().Contains(texto) ||

                    a.AprCorreoPersonal.ToLower().Contains(texto) ||
                    a.AprCorreoInstitucional.ToLower().Contains(texto) ||
                    a.AprTelefono.ToLower().Contains(texto) ||

                    a.AprAcudNombre.ToLower().Contains(texto) ||
                    a.AprAcudApellido.ToLower().Contains(texto) ||
                    a.AprTelefonoAcudiente.ToLower().Contains(texto) ||

                    a.AprEps.ToLower().Contains(texto) ||
                    a.AprPatologia.ToLower().Contains(texto) ||
                    a.AprTipoPoblacion.ToLower().Contains(texto) ||

                    (a.Municipio != null &&
                       (a.Municipio.CiuNombre.ToLower().Contains(texto) ||
                        a.Municipio.Regional.RegNombre.ToLower().Contains(texto)))
                );

            // PRIMERO ejecuta ToListAsync()
            var datos = await query.ToListAsync();

            if (!datos.Any())
                return NotFound("No se encontraron resultados.");

            // LUEGO haces el mapeo
            var resultado = datos.Select(MapearAprendiz);

            return Ok(resultado);
        }



        [HttpGet("buscar")]
        [Authorize(Policy = "AdministradorYPsicologo")]
        public async Task<IActionResult> Buscar([FromQuery] FiltroAprendizDTO f)
        {
            IQueryable<Aprendiz> q = _uow.Aprendiz.Query().Include(x => x.EstadoAprendiz).Include(x => x.Municipio).ThenInclude(m => m.Regional);

            if (f.Codigo.HasValue)
                q = q.Where(x => x.AprCodigo == f.Codigo.Value);

            if (!string.IsNullOrEmpty(f.TipoDocumento))
                q = q.Where(x => x.AprTipoDocumento.ToLower() == f.TipoDocumento.ToLower());

            if (!string.IsNullOrEmpty((f.NroDocumento)))
                q = q.Where(x => x.AprNroDocumento.ToLower() == f.NroDocumento.ToLower());

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

        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet("estadistica/crecimiento-mensual")]
        public async Task<IActionResult> GetCrecimientoMensual()
        {
            var ahora = DateTime.Now;
            var mesActual = ahora.Month;
            var mesAnterior = ahora.AddMonths(-1).Month;

            var query = _uow.Aprendiz.Query()
                .Where(a => a.AprEstadoRegistro == "activo");

            var totalMesActual = await query.CountAsync(a => a.AprFechaCreacion.Month == mesActual);
            var totalMesAnterior = await query.CountAsync(a => a.AprFechaCreacion.Month == mesAnterior);
            var totalGeneral = await query.CountAsync();

            double promedio = totalGeneral / 12.0;

            double porcentaje = totalMesAnterior == 0
                ? 100
                : ((double)(totalMesActual - totalMesAnterior) / totalMesAnterior) * 100;

            return Ok(new
            {
                totalMesActual,
                totalMesAnterior,
                promedioMensual = Math.Round(promedio, 1),
                porcentajeCrecimiento = Math.Round(porcentaje, 2)
            });
        }


        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpGet("estadistica/total-registrados")]
        public async Task<IActionResult> GetTotalAprendicesRegistrados()
        {
            var total = await _uow.Aprendiz
                .Query()
                .Where(a => a.AprEstadoRegistro == "activo")
                .CountAsync();

            return Ok(new
            {
                totalAprendices = total
            });
        }


        [Authorize(Policy = "AdministradorYPsicologo")]
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

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPost("verificar-codigo")]
        public async Task<IActionResult> VerificarCodigo(VerificarCodigoDTO dto)
        {
            var registro = await _uow.VerificationCode
                .ObtenerTodoConCondicion(v => v.Aprendiz.AprNroDocumento == dto.AprendizId && v.Codigo == dto.Codigo && v.Expiration > DateTime.UtcNow);

            if (!registro.Any())
                return BadRequest("Código inválido o expirado");

            var aprendiz = (await _uow.Aprendiz.ObtenerTodoConCondicion(c => c.AprNroDocumento == dto.AprendizId)).FirstOrDefault();
            aprendiz.AprFechaCreacion = DateTime.Now;
            aprendiz.AprEstadoRegistro = "activo";

            _uow.Aprendiz.Actualizar(aprendiz);
            await _uow.SaveChangesAsync();

            return Ok("Cuenta verificada correctamente.");
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPut("completar-informacion")]
        public async Task<IActionResult> CompletarInformacion(string documento, [FromBody] AprendizDTO nuevoAprendiz)
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
             _uow.Aprendiz.Actualizar(resultado);
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
                mensaje = "Se ha completado la información correctamente!",
                resultados
            });
        }

        [AllowAnonymous]
        [HttpPut("cambiar-correo")]
        public async Task<IActionResult> cambiarCorreo(string documento, string correo)
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

        [Authorize(Policy = "AdministradorYAprendiz")]
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
            var token = JwtPasswordHelper.GenerarToken(usuario.AprNroDocumento, "aprendiz");

            var link = $"{token}";

            await _emailService.SendAsync(dto.Correo, "Recuperación de contraseña", $"Copia este Token: {link}");

            return Ok("Se envió un enlace de recuperación al correo.");
        }

        [AllowAnonymous]
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


        [Authorize(Policy = "AdministradorYPsicologo")]
        [HttpPost]
        public async Task<IActionResult> crearRegistro([FromBody] AprendizCompletoDTO aprendizNuevo)
        {
            if (aprendizNuevo == null)
            {
                return BadRequest("El cuerpo no puede estar vacio!");
            }

            var idEncontrado = await _uow.Aprendiz.ObtenerTodoConCondicion(
                e => e.AprNroDocumento == aprendizNuevo.AprNroDocumento
                && e.AprEstadoRegistro == "activo");

            if (idEncontrado.Any())
            {
                return BadRequest("Se encontro un aprendiz con este documento y activo");
            }

            var regNew = new Aprendiz
            {
                AprTipoDocumento = aprendizNuevo.AprTipoDocumento,
                AprNroDocumento = aprendizNuevo.AprNroDocumento,
                AprFechaCreacion = DateTime.Now,
                AprFechaNac = aprendizNuevo.AprFechaNac,
                AprNombre = aprendizNuevo.AprNombre,
                AprSegundoNombre = aprendizNuevo.AprSegundoNombre,
                AprApellido = aprendizNuevo.AprApellido,
                AprSegundoApellido = aprendizNuevo.AprSegundoApellido,
                AprCiudadFk = aprendizNuevo.AprCiudadFk,
                AprDireccion = aprendizNuevo.AprDireccion,
                AprCorreoInstitucional = aprendizNuevo.AprCorreoInstitucional,
                AprCorreoPersonal = aprendizNuevo.AprCorreoPersonal,
                AprTelefono = aprendizNuevo.AprTelefono,
                AprEps = aprendizNuevo.AprEps,
                AprPatologia = aprendizNuevo.AprPatologia,
                AprEstadoAprFk = aprendizNuevo.AprEstadoAprFk,
                AprTipoPoblacion = aprendizNuevo.AprTipoPoblacion,
                AprAcudNombre = aprendizNuevo.AprAcudNombre,
                AprAcudApellido = aprendizNuevo.AprAcudApellido,
                AprTelefonoAcudiente = aprendizNuevo.AprTelefonoAcudiente
            };

            await _uow.Aprendiz.Agregar(regNew);
            await _uow.SaveChangesAsync();

            var aprendizCreado = await _uow.Aprendiz.ObtenerTodoConCondicion(e => e.AprCodigo == regNew.AprCodigo,
                e => e.Include(c => c.EstadoAprendiz),
                e => e.Include(c => c.Municipio.Regional));

            return Ok(new
            {
                mensaje = "Se ha creado correctamente",
                aprendizCreado
            });
        }


        /// <summary>
        /// Edita la información de un aprendiz. Admin/Psicólogo: cualquier aprendiz. Aprendiz: solo su propio perfil.
        /// </summary>
        [HttpPut("editar/{id}")]
        public async Task<IActionResult> EditarInformacion(int id, [FromBody] AprendizEditarDTO dto)
        {
            // Si es Aprendiz, solo puede editar su propio perfil
            if (User.IsInRole(Roles.Aprendiz))
            {
                var miId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("nameid");
                if (string.IsNullOrEmpty(miId) || id.ToString() != miId)
                    return StatusCode(403, "Solo puedes editar tu propio perfil.");
            }

            var aprEncontrado = await _uow.Aprendiz.ObtenerTodoConCondicion(a => a.AprCodigo == id);

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
            resultado.AprFechaNac = dto.AprFechaNac;
            resultado.AprNombre = dto.AprNombre;
            resultado.AprSegundoNombre = dto.AprSegundoNombre;
            resultado.AprApellido = dto.AprApellido;
            resultado.AprSegundoApellido = dto.AprSegundoApellido;
            resultado.AprCiudadFk = dto.AprCiudadFk;
            resultado.AprDireccion = dto.AprDireccion;
            resultado.AprCorreoPersonal = dto.AprCorreoPersonal;
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


        /// <summary>
        /// Cambia el estado del aprendiz (activo/inactivo).
        /// Admin/Psicólogo: pueden activar o inactivar cualquier aprendiz.
        /// Aprendiz: solo puede desactivar su propia cuenta (soft delete - "Eliminar mi cuenta").
        /// </summary>
        [HttpPut("cambiar-estado/{documento}")]
        public async Task<IActionResult> CambiarEstadoAprendiz(string documento, [FromBody] RazonEliminacionDTO dto)
        {
            var aprEncontrado = await _uow.Aprendiz.ObtenerTodoConCondicion(a => a.AprNroDocumento == documento);

            var user = aprEncontrado.FirstOrDefault();

            if (user == null)
                return NotFound("No se encontró este documento.");

            var estadoActual = user.AprEstadoRegistro?.ToLower();

            // Si es Aprendiz, solo puede desactivar su propia cuenta (activo → inactivo)
            if (User.IsInRole(Roles.Aprendiz))
            {
                var idLogueado = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("nameid");
                if (string.IsNullOrEmpty(idLogueado) || user.AprCodigo.ToString() != idLogueado)
                    return StatusCode(403, "No puedes cambiar el estado de otro aprendiz.");

                if (estadoActual == "inactivo")
                    return StatusCode(403, "Solo el administrador o el psicólogo pueden reactivar tu cuenta.");

                // activo → inactivo: desactivar mi cuenta
                if (dto == null || string.IsNullOrWhiteSpace(dto.RazonEliminacion))
                    return BadRequest("Debe enviar una razón de inactivación.");

                user.AprEstadoRegistro = "inactivo";
                user.AprRazonEliminacion = dto.RazonEliminacion;
                user.AprFechaEliminacion = DateTime.Now;
            }
            else
            {
                // Admin o Psicólogo: pueden activar e inactivar
                if (estadoActual == "activo")
                {
                    if (dto == null || string.IsNullOrWhiteSpace(dto.RazonEliminacion))
                        return BadRequest("Debe enviar una razón de inactivación.");

                    user.AprEstadoRegistro = "inactivo";
                    user.AprRazonEliminacion = dto.RazonEliminacion;
                    user.AprFechaEliminacion = DateTime.Now;
                }
                else
                {
                    user.AprEstadoRegistro = "activo";
                    user.AprRazonEliminacion = null;
                    user.AprFechaEliminacion = null;
                }
            }

            _uow.Aprendiz.Actualizar(user);
            await _uow.SaveChangesAsync();
            return Ok($"Estado actualizado a: {user.AprEstadoRegistro}");
        }

        [Authorize(Policy = "SoloAdministrador")]
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarDefinitivo(int id)
        {
            var aprEncontrado = await _uow.Aprendiz.ObtenerPorID(id);
            if (aprEncontrado == null)
            {
                return NotFound("No se encontró este ID");
            }

            _uow.Aprendiz.Eliminar(aprEncontrado);
            await _uow.SaveChangesAsync();
            return Ok("Se ha eliminado correctamente!");
        }
    }
}
