using API_healthyMind.Models;
using System.Linq.Expressions;

namespace API_healthyMind.Repositorios.Interfaces
{
    public interface ICentroRepository : InterfazGenerica<Centro>
    {
        Task<IEnumerable<Centro>> ObtenerConRegional(
            Func<IQueryable<Centro>, IQueryable<Centro>> include);

        Task<IEnumerable<Centro>> ObtenerPorRegional(
            Expression<Func<Centro, bool>> condicion,
            Func<IQueryable<Centro>, IQueryable<Centro>> include);

        Task<IEnumerable<Centro>> ObtenerPorIdWithCondition(
            Expression<Func<Centro, bool>> condicion,
            Func<IQueryable<Centro>, IQueryable<Centro>> include);
    }
}
