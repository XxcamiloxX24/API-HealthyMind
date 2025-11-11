using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class CentroRepository : RepositorioGenerico<Centro>, ICentroRepository
    {
        private readonly DbSet<Centro> _dbSet;
        public CentroRepository(AppDbContext context) : base(context)
        {
            _dbSet = context.Set<Centro>();
        }

        public async Task<IEnumerable<Centro>> ObtenerConRegional(Func<IQueryable<Centro>, IQueryable<Centro>> include)
        {
            IQueryable<Centro> query = _dbSet;
            if (include != null)
            {
                query = include(query);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Centro>> ObtenerPorRegional(Expression<Func<Centro, bool>> condicion, Func<IQueryable<Centro>, IQueryable<Centro>> include)
        {
            IQueryable<Centro> query = _dbSet.Where(condicion);
            if (include != null)
            {
                query = include(query);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Centro>> ObtenerPorIdWithCondition(Expression<Func<Centro, bool>> condicion, Func<IQueryable<Centro>, IQueryable<Centro>> include)
        {
            IQueryable<Centro> query = _dbSet.Where(condicion);
            if (include != null)
            {
                query = include(query);
            }
            return await query.ToListAsync();
        }
    }
}
