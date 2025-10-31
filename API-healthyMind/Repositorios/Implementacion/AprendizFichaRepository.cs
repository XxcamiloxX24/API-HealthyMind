using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class AprendizFichaRepository : RepositorioGenerico<AprendizFicha>, IAprendizFichaRepository
    {
        public AprendizFichaRepository(AppDbContext context) : base(context)
        {
        }
    }
}
