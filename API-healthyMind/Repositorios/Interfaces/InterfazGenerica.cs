using API_healthyMind.Models;
using System.Linq.Expressions;

namespace API_healthyMind.Repositorios.Interfaces
{
    public interface InterfazGenerica<T> where T : class
    {
        IQueryable<T> Query();
        Task<IEnumerable<T>> ObtenerTodoConCondicion(
            Expression<Func<T, bool>> condicion,
            params Expression<Func<IQueryable<T>, IQueryable<T>>>[] includes);
        Task<T?> ObtenerPrimero(Expression<Func<T, bool>> condicion);
        Task<IEnumerable<T>> ObtenerTodos();
        Task<T> ObtenerPorID(int id);
        Task Agregar(T entity);
        void Actualizar(T entity);
        void Eliminar(T id);
    }
}
