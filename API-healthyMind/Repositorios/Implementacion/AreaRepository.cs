using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class AreaRepository : RepositorioGenerico<Area>, IAreaRepository
    {
        private readonly DbSet<Area> _dbSet;
        public AreaRepository(AppDbContext context) : base(context)
        {
            _dbSet = context.Set<Area>();
        }

        public async Task<IEnumerable<Area>> ObtenerTodoConPsic(Expression<Func<Area, bool>> condicion, Func<IQueryable<Area>, IQueryable<Area>> include)
        {
            IQueryable<Area> query = _dbSet.Where(condicion);
            if (include != null)
            {
                query = include(query);
            }
            return await query.ToListAsync();
        }
    }
}
