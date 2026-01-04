using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class CiudadRepository : RepositorioGenerico<Ciudad>, ICiudadRepository
    {
        private readonly DbSet<Ciudad> _dbSet;
        public CiudadRepository(AppDbContext context) : base(context)
        {
            _dbSet = context.Set<Ciudad>();
        }

        

        public async Task<IEnumerable<Ciudad>> ObtenerConRegional(Func<IQueryable<Ciudad>, IQueryable<Ciudad>> include)
        {
            IQueryable<Ciudad> query = _dbSet;
            if (include != null)
            {
                query = include(query);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Ciudad>> ObtenerPorRegional(Expression<Func<Ciudad, bool>> condicion, Func<IQueryable<Ciudad>, IQueryable<Ciudad>> include)
        {
            IQueryable<Ciudad> query = _dbSet.Where(condicion);
            if (include != null)
            {
                query = include(query);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Ciudad>> Buscar(
            Expression<Func<Ciudad, bool>> filtro,
            Func<IQueryable<Ciudad>, IQueryable<Ciudad>> include = null)
        {
            IQueryable<Ciudad> query = _dbSet;

            if (include != null)
                query = include(query);

            return await query.Where(filtro).ToListAsync();
        }
    }
}
