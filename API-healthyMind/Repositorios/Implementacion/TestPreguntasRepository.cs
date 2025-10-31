using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class TestPreguntasRepository : RepositorioGenerico<TestPreguntas>, ITestPreguntasRepository
    {
        public TestPreguntasRepository(AppDbContext context) : base(context)
        {
        }
    }
}
