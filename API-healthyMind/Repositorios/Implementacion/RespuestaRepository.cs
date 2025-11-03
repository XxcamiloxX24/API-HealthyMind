using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class RespuestaRepository : RepositorioGenerico<Respuestas>, IRespuestasRepository
    {
        public RespuestaRepository(AppDbContext context) : base(context)
        {
        }
    }
}
