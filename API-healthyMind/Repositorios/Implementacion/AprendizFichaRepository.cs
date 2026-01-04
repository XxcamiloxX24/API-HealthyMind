using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class AprendizFichaRepository : RepositorioGenerico<AprendizFicha>, IAprendizFichaRepository
    {
        private readonly DbSet<Aprendiz> _dbSet;
        public AprendizFichaRepository(AppDbContext context) : base(context)
        {
            _dbSet = context.Set<Aprendiz>();
        }
        
    }
}
