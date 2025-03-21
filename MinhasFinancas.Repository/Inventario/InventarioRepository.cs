using MinhasFinancas.Infra.Data;
using MinhasFinancas.Repository.Core;

namespace MinhasFinancas.Repository.Inventario
{
    public class InventarioRepository : Repository<Infra.Models.Inventario>, IInventarioRepository
    {
        public InventarioRepository(AppDbContext db) : base(db)
        {
        }
    }
}