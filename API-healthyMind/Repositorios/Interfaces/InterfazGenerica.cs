namespace API_healthyMind.Repositorios.Interfaces
{
    public interface InterfazGenerica<T> where T : class
    {
        Task<IEnumerable<T>> ObtenerTodos();
        Task<T> ObtenerPorID(int id);
        Task Agregar(T entity);
        void Actualizar(T entity);
        void Eliminar(T id);
    }
}
