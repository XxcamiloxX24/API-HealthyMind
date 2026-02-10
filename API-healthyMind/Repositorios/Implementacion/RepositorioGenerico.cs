using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class RepositorioGenerico<T> : InterfazGenerica<T> where T : class
    {
        protected readonly AppDbContext _appDbContext;
        protected readonly DbSet<T> _dbset;

        public RepositorioGenerico(AppDbContext context)
        {
            _appDbContext = context;
            _dbset = context.Set<T>();
        }

        public IQueryable<T> Query()
        {
            return _dbset.AsQueryable();
        }

        public async Task<IEnumerable<T>> ObtenerTodoConCondicion(
            Expression<Func<T, bool>> condicion,
            params Expression<Func<IQueryable<T>, IQueryable<T>>>[] includes)
        {
            IQueryable<T> query = _dbset.Where(condicion);
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = include.Compile()(query);
                }
            }

            return await query.ToListAsync();
        }
        public async Task<T?> ObtenerPrimero(Expression<Func<T, bool>> condicion)
        {
            return await _dbset.FirstOrDefaultAsync(condicion);
        }
        public async Task<IEnumerable<T>> ObtenerTodos() => await _dbset.ToListAsync();
        public async Task<T> ObtenerPorID(int id) => await _dbset.FindAsync(id);
        public async Task Agregar(T entity) => await _dbset.AddAsync(entity);
        public void Actualizar(T entity) => _dbset.Update(entity);
        public void Eliminar(T id) => _dbset.Remove(id);




    }
}
