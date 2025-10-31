using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class FichaRepository : RepositorioGenerico<Ficha>, IFichaRepository
    {
        public FichaRepository(AppDbContext context) : base(context)
        {
        }
    }
}
