using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class PlantillaOpcionRepository : RepositorioGenerico<PlantillaOpcion>, IPlantillaOpcionRepository
    {
        public PlantillaOpcionRepository(AppDbContext context) : base(context)
        {
        }
    }
}
