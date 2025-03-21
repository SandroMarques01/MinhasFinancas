using MinhasFinancas.Infra.Data;
using MinhasFinancas.Repository.Core;

namespace MinhasFinancas.Repository.Ativo
{
    public class AtivoRepository : Repository<Infra.Models.Ativo>, IAtivoRepository
    {
        public AtivoRepository(AppDbContext db) : base(db)
        {
        }
    }
}