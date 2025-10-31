using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class TestGeneralRepository : RepositorioGenerico<TestGeneral>, ITestGeneralRepository
    {
        public TestGeneralRepository(AppDbContext context) : base(context)
        {
        }
    }
}
