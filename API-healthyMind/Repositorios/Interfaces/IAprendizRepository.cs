using API_healthyMind.Models;
using System.Linq.Expressions;

namespace API_healthyMind.Repositorios.Interfaces
{
    public interface IAprendizRepository : InterfazGenerica<Aprendiz>
    {
        Task<bool> Existe(Expression<Func<Aprendiz, bool>> predicado);

        Task<IEnumerable<Aprendiz>> Buscar(
            Expression<Func<Aprendiz, bool>> filtro,
            Func<IQueryable<Aprendiz>, IQueryable<Aprendiz>> include = null
        );

    }
}
