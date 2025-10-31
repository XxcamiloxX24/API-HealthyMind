using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class CiudadRepository : RepositorioGenerico<Ciudad>, ICiudadRepository
    {
        public CiudadRepository(AppDbContext context) : base(context)
        {
        }
    }
}
