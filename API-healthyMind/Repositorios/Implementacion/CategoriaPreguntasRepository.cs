using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class CategoriaPreguntasRepository : RepositorioGenerico<CategoriaPreguntas>, ICategoriaPreguntasRepository
    {
        public CategoriaPreguntasRepository(AppDbContext context) : base(context)
        {
        }
    }
}
