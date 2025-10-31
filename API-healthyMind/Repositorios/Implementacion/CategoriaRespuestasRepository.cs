using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class CategoriaRespuestasRepository : RepositorioGenerico<CategoriaRespuestas>, ICategoriaRespuestasRepository
    {
        public CategoriaRespuestasRepository(AppDbContext context) : base(context)
        {
        }
    }
}
