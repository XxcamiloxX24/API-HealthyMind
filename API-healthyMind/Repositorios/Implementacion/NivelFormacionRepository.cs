using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class NivelFormacionRepository : RepositorioGenerico<NivelFormacion>, INivelFormacionRepository
    {
        public NivelFormacionRepository(AppDbContext context) : base(context)
        {
        }
    }
}
