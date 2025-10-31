using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class PaginaDiarioRepository : RepositorioGenerico<PaginaDiario>, IPaginaDiarioRepository
    {
        public PaginaDiarioRepository(AppDbContext context) : base(context)
        {
        }
    }
}
