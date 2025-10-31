using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class ProgramaFormacionRepository : RepositorioGenerico<Programaformacion>, IProgramaFormacionRepository
    {
        public ProgramaFormacionRepository(AppDbContext context) : base(context)
        {
        }
    }
}
