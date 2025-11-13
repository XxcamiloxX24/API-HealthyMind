using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class FichaRepository : RepositorioGenerico<Ficha>, IFichaRepository
    {
        private readonly DbSet<Ficha> _dbSet;
        public FichaRepository(AppDbContext context) : base(context)
        {
            _dbSet = context.Set<Ficha>();
        }

        public async Task<IEnumerable<Ficha>> ObtenerTodoConCondicion(
            Expression<Func<Ficha, bool>> condicion, 
            params Expression<Func<IQueryable<Ficha>, IQueryable<Ficha>>>[] includes)
        {
            IQueryable<Ficha> query = _dbSet.Where(condicion);
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = include.Compile()(query);
                }
            }

            return await query.ToListAsync();
        }


    }
}
