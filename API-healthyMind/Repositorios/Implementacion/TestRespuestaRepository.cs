using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class TestRespuestaRepository : RepositorioGenerico<TestRespuesta>, ITestRespuestaRepository
    {
        public TestRespuestaRepository(AppDbContext context) : base(context)
        {
        }
    }
}
