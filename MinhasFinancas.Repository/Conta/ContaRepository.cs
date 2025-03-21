using MinhasFinancas.Infra.Data;
using MinhasFinancas.Repository.Core;

namespace MinhasFinancas.Repository.Conta
{
    public class ContaRepository : Repository<Infra.Models.Conta>, IContaRepository
    {
        public ContaRepository(AppDbContext db) : base(db)
        {
        }
    }
}