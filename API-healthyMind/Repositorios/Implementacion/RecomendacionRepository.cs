using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion;

public class RecomendacionRepository : RepositorioGenerico<Recomendacion>, IRecomendacionRepository
{
    public RecomendacionRepository(AppDbContext context) : base(context)
    {
    }
}
