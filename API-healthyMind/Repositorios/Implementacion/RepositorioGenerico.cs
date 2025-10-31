using API_healthyMind.Data;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<T>> ObtenerTodos() => await _dbset.ToListAsync();
        public async Task<T> ObtenerPorID(int id) => await _dbset.FindAsync(id);
        public async Task Agregar(T entity) => await _dbset.AddAsync(entity);
        public void Actualizar(T entity) => _dbset.Update(entity);
        public void Eliminar(T id) => _dbset.Remove(id);




    }
}
