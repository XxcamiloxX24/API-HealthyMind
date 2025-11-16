using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class AprendizRepository : RepositorioGenerico<Aprendiz>, IAprendizRepository
    {
        private readonly DbSet<Aprendiz> _dbSet;
        public AprendizRepository(AppDbContext context) : base(context)
        {
            _dbSet = context.Set<Aprendiz>();
        }

        public async Task<bool> Existe(Expression<Func<Aprendiz, bool>> predicado)
        {
            return await _dbset.AnyAsync(predicado);
        }
    }
}
