using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class CentroRepository : RepositorioGenerico<Centro>, ICentroRepository
    {
        public CentroRepository(AppDbContext context) : base(context)
        {
        }
    }
}
