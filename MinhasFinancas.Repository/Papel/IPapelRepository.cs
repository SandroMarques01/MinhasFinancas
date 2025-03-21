using MinhasFinancas.Repository.Core;
using System;
using System.Threading.Tasks;

namespace MinhasFinancas.Repository.Papel
{
    public interface IPapelRepository : IRepository<Infra.Models.Papel>
    {
        Task DeleteAllByUser(string userId);
    }
}
