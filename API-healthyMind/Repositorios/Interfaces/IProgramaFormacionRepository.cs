using API_healthyMind.Models;
using System.Linq.Expressions;

namespace API_healthyMind.Repositorios.Interfaces
{
    public interface IProgramaFormacionRepository : InterfazGenerica<Programaformacion>
    {
        Task<IEnumerable<Programaformacion>> ObtenerTodoConCondicion(
            Expression<Func<Programaformacion, bool>> condicion,
            params Expression<Func<IQueryable<Programaformacion>, IQueryable<Programaformacion>>>[] includes);
    }
}
