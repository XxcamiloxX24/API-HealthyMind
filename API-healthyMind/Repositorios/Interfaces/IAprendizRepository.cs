using API_healthyMind.Models;
using System.Linq.Expressions;

namespace API_healthyMind.Repositorios.Interfaces
{
    public interface IAprendizRepository : InterfazGenerica<Aprendiz>
    {
        Task<bool> Existe(Expression<Func<Aprendiz, bool>> predicado);

    }
}
