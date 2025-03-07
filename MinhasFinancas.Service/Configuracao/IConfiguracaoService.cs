﻿using System;
using System.Threading.Tasks;
using System.Web;

namespace MinhasFinancas.Service.Configuracao
{
    public interface IConfiguracaoService : IDisposable
    {
        Task ImportarExcelB3(HttpPostedFileBase fileB3, string userId);
        Task ImportarExcelCotacaoAtual(HttpPostedFileBase fileB3, string userId);
        Task DeletaTodoBanco(string userId);
    }
}
