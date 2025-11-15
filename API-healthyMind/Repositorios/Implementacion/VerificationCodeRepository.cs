using API_healthyMind.Data;
using API_healthyMind.Models;
using API_healthyMind.Repositorios.Interfaces;

namespace API_healthyMind.Repositorios.Implementacion
{
    public class VerificationCodeRepository : RepositorioGenerico<VerificationCode>, IVerificationCodeRepository
    {
        private readonly AppDbContext _context;

        public VerificationCodeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
