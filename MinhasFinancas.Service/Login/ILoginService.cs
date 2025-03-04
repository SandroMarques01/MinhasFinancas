using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MinhasFinancas.Service.Login
{
    public interface ILoginService : IDisposable
    {
        Task Add(Infra.Models.Login obj);
        Task Update(Infra.Models.Login obj);
        Task DeleteById(Guid id);
        Task<IEnumerable<Infra.Models.Login>> Get(Expression<Func<Infra.Models.Login, bool>> filter = null, string includeProperties = null);
        Task<Infra.Models.Login> GetById(Guid id);
        Task<Infra.Models.Login> GetByUsuarioSenha(string usuario, string senha);

    }
}
