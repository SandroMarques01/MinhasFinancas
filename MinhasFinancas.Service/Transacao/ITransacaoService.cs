using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;

namespace MinhasFinancas.Service.Transacao
{
    public interface ITransacaoService : IDisposable
    {
        Task Add(Infra.Models.Transacao obj);
        Task Update(Infra.Models.Transacao obj);
        Task UpdateTrocaPapel(Infra.Models.Transacao obj);
        Task DeleteById(Guid id);
        Task<IEnumerable<Infra.Models.Transacao>> Get(Expression<Func<Infra.Models.Transacao, bool>> filter = null, string includeProperties = null);
        Task<Infra.Models.Transacao> GetById(Guid id);
        Task DeleteAllByUser(string userId);
    }
}
