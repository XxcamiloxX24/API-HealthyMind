using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class EstadoAprendizRepository : RepositorioGenerico<EstadoAprendiz>, IEstadoAprendizRepository
    {
        public EstadoAprendizRepository(AppDbContext context) : base(context)
        {
        }
    }
}
