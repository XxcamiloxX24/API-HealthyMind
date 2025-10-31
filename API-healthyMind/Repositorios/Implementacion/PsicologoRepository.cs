using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class PsicologoRepository : RepositorioGenerico<Psicologo>, IPsicologoRepository
    {
        public PsicologoRepository(AppDbContext context) : base(context)
        {
        }
    }
}
