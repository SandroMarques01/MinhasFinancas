using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MinhasFinancas.Service.Ativo
{
    public interface IAtivoService : IDisposable
    {
        Task Add(Infra.Models.Ativo obj);
        Task Update(Infra.Models.Ativo obj);
        Task DeleteById(Guid id);
        Task<IEnumerable<Infra.Models.Ativo>> Get(Expression<Func<Infra.Models.Ativo, bool>> filter = null, string includeProperties = null);
        Task<Infra.Models.Ativo> GetById(Guid id);

    }
}
