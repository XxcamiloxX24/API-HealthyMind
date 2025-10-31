using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class EmocionesRepository : RepositorioGenerico<Emociones>, IEmocionesRepository
    {
        public EmocionesRepository(AppDbContext context) : base(context)
        {
        }
    }
}
