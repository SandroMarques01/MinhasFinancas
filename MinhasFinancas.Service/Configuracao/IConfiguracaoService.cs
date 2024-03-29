﻿using System;
using System.Threading.Tasks;
using System.Web;

namespace MinhasFinancas.Service.Configuracao
{
    public interface IConfiguracaoService : IDisposable
    {
        Task ImportarExcelB3(HttpPostedFileBase fileB3);
        Task ImportarExcelCotacaoAtual(HttpPostedFileBase fileB3);
        Task DeletaTodoBanco();
    }
}
