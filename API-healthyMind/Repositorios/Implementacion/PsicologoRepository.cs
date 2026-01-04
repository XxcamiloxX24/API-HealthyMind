using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class PsicologoRepository : RepositorioGenerico<Psicologo>, IPsicologoRepository
    {
        private readonly DbSet<Psicologo> _dbSet;
        public PsicologoRepository(AppDbContext context) : base(context)
        {
            _dbSet = context.Set<Psicologo>();
        }

        public async Task<IEnumerable<Psicologo>> Buscar(
            Expression<Func<Psicologo, bool>> filtro,
            Func<IQueryable<Psicologo>, IQueryable<Psicologo>> include = null)
        {
            IQueryable<Psicologo> query = _dbSet;

            if (include != null)
                query = include(query);

            return await query.Where(filtro).ToListAsync();
        }
    }
}
