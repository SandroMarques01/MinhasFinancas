using MinhasFinancas.Repository.Core;
using System;
using System.Threading.Tasks;

namespace MinhasFinancas.Repository.Transacao
{
    public interface ITransacaoRepository : IRepository<Infra.Models.Transacao>
    {
        Task DeleteAllByUser(string userId);
    }
}
