using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class PreguntasRepository : RepositorioGenerico<Preguntas>, IPreguntasRepository
    {
        public PreguntasRepository(AppDbContext context) : base(context)
        {
        }
    }
}
