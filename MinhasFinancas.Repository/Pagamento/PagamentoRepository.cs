using MinhasFinancas.Infra.Data;
using MinhasFinancas.Repository.Core;

namespace MinhasFinancas.Repository.Pagamento
{
    public class PagamentoRepository : Repository<Infra.Models.Pagamento>, IPagamentoRepository
    {
        public PagamentoRepository(AppDbContext db) : base(db)
        {
        }
    }
}