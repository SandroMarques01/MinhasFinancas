using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;

namespace MinhasFinancas.Service.Papel
{
    public interface IPapelService : IDisposable
    {
        Task Add(Infra.Models.Papel obj);
        Task Update(Infra.Models.Papel obj);
        Task DeleteById(Guid id);
        Task<IEnumerable<Infra.Models.Papel>> Get(Expression<Func<Infra.Models.Papel, bool>> filter = null, string includeProperties = null);
        Task<Infra.Models.Papel> GetById(Guid id);
        Task TrocaPapel (Guid origem,  Guid destino);
        Task Desdobramento(Guid origem, int quantidade, DateTime dataDesdobro);
        Task<IEnumerable<Infra.Models.Papel>> GetDividendosSemana(string includeProperties = null);

    }
}
