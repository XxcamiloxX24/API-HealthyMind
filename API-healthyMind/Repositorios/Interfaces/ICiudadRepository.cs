using API_healthyMind.Models;
using System.Linq.Expressions;

namespace API_healthyMind.Repositorios.Interfaces
{
    public interface ICiudadRepository : InterfazGenerica<Ciudad>
    {
        Task<IEnumerable<Ciudad>> ObtenerConRegional(
            Func<IQueryable<Ciudad>, IQueryable<Ciudad>> include);

        Task<IEnumerable<Ciudad>> ObtenerPorRegional(
            Expression<Func<Ciudad, bool>> condicion,
            Func<IQueryable<Ciudad>, IQueryable<Ciudad>> include);
    }
}
