using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class CardsInfoRepository : RepositorioGenerico<CardsInfo>, ICardsInfoRepository
    {
        public CardsInfoRepository(AppDbContext context) : base(context)
        {
        }
    }
}
