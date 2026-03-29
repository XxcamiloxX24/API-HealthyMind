using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class PlantillaPreguntaRepository : RepositorioGenerico<PlantillaPregunta>, IPlantillaPreguntaRepository
    {
        public PlantillaPreguntaRepository(AppDbContext context) : base(context)
        {
        }
    }
}
