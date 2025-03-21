using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MinhasFinancas.Service.Conta
{
    public interface IContaService : IDisposable
    {
        Task Add(Infra.Models.Conta obj);
        Task Update(Infra.Models.Conta obj);
        Task DeleteById(Guid id);
        Task<IEnumerable<Infra.Models.Conta>> Get(Expression<Func<Infra.Models.Conta, bool>> filter = null, string includeProperties = null);
        Task<Infra.Models.Conta> GetById(Guid id);

    }
}