using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class CitasRepository : RepositorioGenerico<Cita>, ICitasRepository
    {
        public CitasRepository(AppDbContext context) : base(context)
        {
        }
    }
}
