using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class DiarioRepository : RepositorioGenerico<Diario>, IDiarioRepository
    {
        public DiarioRepository(AppDbContext context) : base(context)
        {
        }
    }
}
