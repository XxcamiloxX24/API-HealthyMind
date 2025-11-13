using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class ProgramaFormacionRepository : RepositorioGenerico<Programaformacion>, IProgramaFormacionRepository
    {
        private readonly DbSet<Programaformacion> _dbSet;
        public ProgramaFormacionRepository(AppDbContext context) : base(context)
        {
            _dbSet = context.Set<Programaformacion>();
        }

        public async Task<IEnumerable<Programaformacion>> ObtenerTodoConCondicion(
            Expression<Func<Programaformacion, bool>> condicion,
            params Expression<Func<IQueryable<Programaformacion>, IQueryable<Programaformacion>>>[] includes)
        {
            IQueryable<Programaformacion> query = _dbSet.Where(condicion);

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
