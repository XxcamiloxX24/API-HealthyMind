using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_healthyMind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.Psicologo)]
    public class PlantillaTestController : ControllerBase
    {
        private readonly IUnidadDeTrabajo _uow;

        public PlantillaTestController(IUnidadDeTrabajo uow)
        {
            _uow = uow;
        }

        private bool TryObtenerPsicologoIdAutenticado(out int psicologoId)
        {
            psicologoId = 0;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("nameid");
            return !string.IsNullOrWhiteSpace(userId) && int.TryParse(userId, out psicologoId);
        }

        /// <summary>
        /// Crea una plantilla completa con preguntas y opciones en un solo request.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] PlantillaTestDTO dto)
        {
            if (!TryObtenerPsicologoIdAutenticado(out var psicologoId))
                return Forbid();

            if (string.IsNullOrWhiteSpace(dto.PlaTstNombre))
                return BadRequest(new { message = "El nombre de la plantilla es obligatorio." });

            var plantilla = new PlantillaTest
            {
                PlaTstNombre = dto.PlaTstNombre.Trim(),
                PlaTstDescripcion = dto.PlaTstDescripcion?.Trim(),
                PlaTstPsicologoFk = psicologoId,
                PlaTstFechaCreacion = DateTime.UtcNow
            };

            if (dto.Preguntas != null)
            {
                foreach (var pDto in dto.Preguntas)
                {
                    var pregunta = new PlantillaPregunta
                    {
                        PlaPrgTexto = pDto.PlaPrgTexto.Trim(),
                        PlaPrgTipo = pDto.PlaPrgTipo?.Trim() ?? "opcion_multiple",
                        PlaPrgOrden = pDto.PlaPrgOrden
                    };

                    if (pDto.Opciones != null)
                    {
                        foreach (var oDto in pDto.Opciones)
                        {
                            pregunta.Opciones.Add(new PlantillaOpcion
                            {
                                PlaOpcTexto = oDto.PlaOpcTexto.Trim(),
                                PlaOpcOrden = oDto.PlaOpcOrden
                            });
                        }
                    }

                    plantilla.Preguntas.Add(pregunta);
                }
            }

            await _uow.PlantillaTest.Agregar(plantilla);
            await _uow.SaveChangesAsync();

            return Ok(new { message = "Plantilla creada correctamente.", plantillaId = plantilla.PlaTstCodigo });
        }

        /// <summary>
        /// Lista las plantillas del psicólogo autenticado.
        /// </summary>
        [HttpGet("mis-plantillas")]
        public async Task<IActionResult> MisPlantillas()
        {
            if (!TryObtenerPsicologoIdAutenticado(out var psicologoId))
                return Forbid();

            var datos = await _uow.PlantillaTest.Query()
                .Where(p => p.PlaTstEstadoRegistro == "activo" && p.PlaTstPsicologoFk == psicologoId)
                .Include(p => p.Preguntas.Where(q => q.PlaPrgEstadoRegistro == "activo"))
                .OrderByDescending(p => p.PlaTstFechaCreacion)
                .Select(p => new
                {
                    p.PlaTstCodigo,
                    p.PlaTstNombre,
                    p.PlaTstDescripcion,
                    p.PlaTstFechaCreacion,
                    TotalPreguntas = p.Preguntas.Count(q => q.PlaPrgEstadoRegistro == "activo")
                })
                .ToListAsync();

            return Ok(datos);
        }

        /// <summary>
        /// Detalle de una plantilla con preguntas y opciones.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            if (!TryObtenerPsicologoIdAutenticado(out var psicologoId))
                return Forbid();

            var plantilla = await _uow.PlantillaTest.Query()
                .Where(p => p.PlaTstCodigo == id && p.PlaTstEstadoRegistro == "activo" && p.PlaTstPsicologoFk == psicologoId)
                .Include(p => p.Preguntas.Where(q => q.PlaPrgEstadoRegistro == "activo"))
                    .ThenInclude(q => q.Opciones.Where(o => o.PlaOpcEstadoRegistro == "activo"))
                .FirstOrDefaultAsync();

            if (plantilla == null)
                return NotFound("Plantilla no encontrada.");

            return Ok(new
            {
                plantilla.PlaTstCodigo,
                plantilla.PlaTstNombre,
                plantilla.PlaTstDescripcion,
                plantilla.PlaTstFechaCreacion,
                Preguntas = plantilla.Preguntas
                    .OrderBy(q => q.PlaPrgOrden)
                    .Select(q => new
                    {
                        q.PlaPrgCodigo,
                        q.PlaPrgTexto,
                        q.PlaPrgTipo,
                        q.PlaPrgOrden,
                        Opciones = q.Opciones
                            .OrderBy(o => o.PlaOpcOrden)
                            .Select(o => new
                            {
                                o.PlaOpcCodigo,
                                o.PlaOpcTexto,
                                o.PlaOpcOrden
                            })
                    })
            });
        }

        /// <summary>
        /// Edita una plantilla (reemplaza preguntas y opciones).
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] PlantillaTestDTO dto)
        {
            if (!TryObtenerPsicologoIdAutenticado(out var psicologoId))
                return Forbid();

            var plantilla = await _uow.PlantillaTest.Query()
                .Where(p => p.PlaTstCodigo == id && p.PlaTstEstadoRegistro == "activo" && p.PlaTstPsicologoFk == psicologoId)
                .Include(p => p.Preguntas)
                    .ThenInclude(q => q.Opciones)
                .FirstOrDefaultAsync();

            if (plantilla == null)
                return NotFound("Plantilla no encontrada.");

            plantilla.PlaTstNombre = dto.PlaTstNombre?.Trim() ?? plantilla.PlaTstNombre;
            plantilla.PlaTstDescripcion = dto.PlaTstDescripcion?.Trim();

            foreach (var preg in plantilla.Preguntas)
            {
                preg.PlaPrgEstadoRegistro = "inactivo";
                foreach (var opc in preg.Opciones)
                    opc.PlaOpcEstadoRegistro = "inactivo";
            }

            if (dto.Preguntas != null)
            {
                foreach (var pDto in dto.Preguntas)
                {
                    var pregunta = new PlantillaPregunta
                    {
                        PlaPrgTexto = pDto.PlaPrgTexto.Trim(),
                        PlaPrgTipo = pDto.PlaPrgTipo?.Trim() ?? "opcion_multiple",
                        PlaPrgOrden = pDto.PlaPrgOrden,
                        PlaPrgPlantillaFk = plantilla.PlaTstCodigo
                    };

                    if (pDto.Opciones != null)
                    {
                        foreach (var oDto in pDto.Opciones)
                        {
                            pregunta.Opciones.Add(new PlantillaOpcion
                            {
                                PlaOpcTexto = oDto.PlaOpcTexto.Trim(),
                                PlaOpcOrden = oDto.PlaOpcOrden
                            });
                        }
                    }

                    var ctx = _uow.ObtenerContexto();
                    await ctx.PlantillaPreguntas.AddAsync(pregunta);
                }
            }

            _uow.PlantillaTest.Actualizar(plantilla);
            await _uow.SaveChangesAsync();

            return Ok(new { message = "Plantilla actualizada correctamente." });
        }

        /// <summary>
        /// Soft delete de la plantilla.
        /// </summary>
        [HttpPut("eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (!TryObtenerPsicologoIdAutenticado(out var psicologoId))
                return Forbid();

            var plantilla = await _uow.PlantillaTest.Query()
                .FirstOrDefaultAsync(p => p.PlaTstCodigo == id && p.PlaTstEstadoRegistro == "activo" && p.PlaTstPsicologoFk == psicologoId);

            if (plantilla == null)
                return NotFound("Plantilla no encontrada.");

            plantilla.PlaTstEstadoRegistro = "inactivo";
            _uow.PlantillaTest.Actualizar(plantilla);
            await _uow.SaveChangesAsync();

            return Ok("Plantilla eliminada correctamente.");
        }
    }
}
