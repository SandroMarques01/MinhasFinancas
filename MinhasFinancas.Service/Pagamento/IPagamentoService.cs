using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MinhasFinancas.Service.Pagamento
{
    public interface IPagamentoService : IDisposable
    {
        Task Add(Infra.Models.Pagamento obj);
        Task Update(Infra.Models.Pagamento obj);
        Task DeleteById(Guid id);
        Task<IEnumerable<Infra.Models.Pagamento>> Get(Expression<Func<Infra.Models.Pagamento, bool>> filter = null, string includeProperties = null);
        Task<Infra.Models.Pagamento> GetById(Guid id);

    }
}