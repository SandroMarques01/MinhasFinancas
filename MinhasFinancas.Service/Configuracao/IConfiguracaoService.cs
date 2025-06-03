using MinhasFinancas.Infra.Models.Partial;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace MinhasFinancas.Service.Configuracao
{
    public interface IConfiguracaoService : IDisposable
    {
        Task ImportarExcelB3(List<ExcelB3> lstB3, string userId);
        Task ImportarExcelCotacaoAtual(HttpPostedFileBase fileB3, string userId);
        Task DeletaTodoBanco(string userId);
    }
}
