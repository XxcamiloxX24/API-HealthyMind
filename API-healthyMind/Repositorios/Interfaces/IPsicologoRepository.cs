using API_healthyMind.Models;
using System.Linq.Expressions;

namespace API_healthyMind.Repositorios.Interfaces
{
    public interface IPsicologoRepository : InterfazGenerica<Psicologo>
    {
        Task<IEnumerable<Psicologo>> Buscar(
            Expression<Func<Psicologo, bool>> filtro,
            Func<IQueryable<Psicologo>, IQueryable<Psicologo>> include = null
        );
    }
}
