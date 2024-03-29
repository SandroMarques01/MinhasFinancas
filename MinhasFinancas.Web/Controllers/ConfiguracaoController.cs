﻿using ClosedXML.Excel;
using MinhasFinancas.Service.Configuracao;
using MinhasFinancas.Service.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MinhasFinancas.Web.Controllers
{
    public class ConfiguracaoController : BaseController
    {
        IConfiguracaoService _configuracaoService;

        public ConfiguracaoController(IConfiguracaoService configuracaoService, 
                                        INotificador notificador) : base(notificador)
        {
            _configuracaoService = configuracaoService;
        }

        // GET: Configuracao
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Importar(HttpPostedFileBase fileB3)
        {
            // Verifica se o arquivo é nulo
            if (fileB3 == null || fileB3.ContentLength == 0)
            {
                throw new ArgumentException("O arquivo é nulo ou vazio.");
            }

            // Verifica se o arquivo é do tipo Excel
            if (!fileB3.ContentType.Equals("application/vnd.ms-excel") &&
                !fileB3.ContentType.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
            {
                throw new ArgumentException("O arquivo não é do tipo Excel.");
            }

            await _configuracaoService.ImportarExcelB3(fileB3);

            return RedirectToAction("Index","Papel");
        }

        [HttpPost]
        public async Task<ActionResult> ImportarCotacaoAtual(HttpPostedFileBase fileCotacaoAtual)
        {
            //Verifica se o arquivo é nulo
            if (fileCotacaoAtual == null || fileCotacaoAtual.ContentLength == 0)
            {
                throw new ArgumentException("O arquivo é nulo ou vazio.");
            }

            // Verifica se o arquivo é do tipo Excel
            if (!fileCotacaoAtual.ContentType.Equals("application/vnd.ms-excel") &&
                !fileCotacaoAtual.ContentType.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
            {
                throw new ArgumentException("O arquivo não é do tipo Excel.");
            }

            await _configuracaoService.ImportarExcelCotacaoAtual(fileCotacaoAtual);

            return RedirectToAction("Index", "Papel");
        }

        public async Task<ActionResult> DeletarBanco()
        {
            await _configuracaoService.DeletaTodoBanco();

            return RedirectToAction("Index", "Home");
        }
    }
}