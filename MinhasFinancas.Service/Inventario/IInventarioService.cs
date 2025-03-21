using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MinhasFinancas.Service.Inventario
{
    public interface IInventarioService : IDisposable
    {
        Task Add(Infra.Models.Inventario obj);
        Task Update(Infra.Models.Inventario obj);
        Task DeleteById(Guid id);
        Task<IEnumerable<Infra.Models.Inventario>> Get(Expression<Func<Infra.Models.Inventario, bool>> filter = null, string includeProperties = null);
        Task<Infra.Models.Inventario> GetById(Guid id);

    }
}