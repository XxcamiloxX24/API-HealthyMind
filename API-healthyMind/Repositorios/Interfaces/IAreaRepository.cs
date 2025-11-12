using API_healthyMind.Models;
using System.Linq.Expressions;

namespace API_healthyMind.Repositorios.Interfaces
{
    public interface IAreaRepository : InterfazGenerica<Area>
    {
        Task<IEnumerable<Area>> ObtenerTodoConPsic(
            Expression<Func<Area, bool>> condicion,
            Func<IQueryable<Area>, IQueryable<Area>> include);
    }
}
