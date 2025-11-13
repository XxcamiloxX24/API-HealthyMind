using API_healthyMind.Models;
using System.Linq.Expressions;

namespace API_healthyMind.Repositorios.Interfaces
{
    public interface IFichaRepository : InterfazGenerica<Ficha>
    {
        Task<IEnumerable<Ficha>> ObtenerTodoConCondicion(
            Expression<Func<Ficha, bool>> condicion,
            params Expression<Func<IQueryable<Ficha>, IQueryable<Ficha>>>[] includes);
    }
}