using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class SeguimientoAprendizRepository : RepositorioGenerico<SeguimientoAprendiz>, ISeguimientoAprendizRepository
    {
        public SeguimientoAprendizRepository(AppDbContext context) : base(context)
        {
        }
    }
}
