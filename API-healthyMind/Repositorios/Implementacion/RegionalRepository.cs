using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class RegionalRepository : RepositorioGenerico<Regional>, IRegionalRepository
    {
        public RegionalRepository(AppDbContext context) : base(context)
        {
        }
    }
}
