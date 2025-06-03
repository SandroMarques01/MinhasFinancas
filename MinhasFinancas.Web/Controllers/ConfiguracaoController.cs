using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using MinhasFinancas.Infra;
using MinhasFinancas.Infra.Models;
using MinhasFinancas.Infra.Models.Partial;
using MinhasFinancas.Service.Configuracao;
using MinhasFinancas.Service.Core;
using MinhasFinancas.Service.Dividendo;
using MinhasFinancas.Service.Login;
using MinhasFinancas.Service.Papel;
using MinhasFinancas.Service.Transacao;
using MinhasFinancas.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static System.Net.WebRequestMethods;

namespace MinhasFinancas.Web.Controllers
{
    public class ConfiguracaoController : BaseController
    {
        IConfiguracaoService _configuracaoService;
        IPapelService _papelService;
        ITransacaoService _transacaoService;
        IDividendoService _dividendoService;
        ILoginService _loginService;
        IMapper _mapper;

        public ConfiguracaoController(IConfiguracaoService configuracaoService,
                                        IPapelService papelService,
                                        ITransacaoService transacaoService,
                                        IDividendoService dividendoService,
                                        ILoginService loginService,
                                        IMapper mapper,
                                        INotificador notificador) : base(notificador)
        {
            _configuracaoService = configuracaoService;
            _papelService = papelService;
            _transacaoService = transacaoService;
            _dividendoService = dividendoService;
            _loginService = loginService;
            _mapper = mapper;
        }

        // GET: Configuracao
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(userId))
                return Redirect(@"/Login/Index");

            return View();
        }

        public async Task<ActionResult> AlterarSenha()
        {
            if (string.IsNullOrEmpty(userId))
                return Redirect(@"/Login/Index");

            LoginViewModel loginViewModel = _mapper.Map<LoginViewModel>(await _loginService.GetById(new Guid(userId)));
            if (loginViewModel == null)
            {
                return HttpNotFound();
            }

            return View(loginViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AlterarSenha(Guid id, LoginViewModel loginViewModel)
        {
            if (id != loginViewModel.Id) return HttpNotFound();

            if (!ModelState.IsValid) return View(loginViewModel);

            Login log = _mapper.Map<Login>(loginViewModel);
            await _loginService.Update(log);

            if (!OperacaoValida()) return View(_mapper.Map<PapelViewModel>(await _loginService.GetById(id)));

            return RedirectToAction("Index");
        }

        public ActionResult Sair()
        {
            ConfigurationManager.AppSettings.Set("User", null);
            ConfigurationManager.AppSettings.Set("UserId", null);
            return Redirect(@"/Login/Index");
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

            var workbook = new XLWorkbook(fileB3.InputStream);
            var planilha = workbook.Worksheets.FirstOrDefault();

            List<ExcelB3> lstB3 = new List<ExcelB3>();

            if (planilha.Name == "Movimentação")
            {
                lstB3 = await RetornaListaExcelB3(planilha);
            }
            else if (planilha.Name == "Proventos a Receber")
            {
                lstB3 = await RetornaListaDividendosExcelB3(planilha);
            }

            await _configuracaoService.ImportarExcelB3(lstB3.OrderBy(o => o.Data).ToList(), userId);


            return RedirectToAction("Index", "Papel");
        }

        private async Task<List<ExcelB3>> RetornaListaExcelB3(IXLWorksheet planilha)
        {
            List<ExcelB3> lst = new List<ExcelB3>();

            var totalLinhas = planilha.Rows().Count();

            for (int l = 2; l <= totalLinhas; l++)
            {
                lst.Add(new ExcelB3
                {
                    EntradaSaida = planilha.Cell($"A{l}").Value.ToString(),
                    Data = planilha.Cell($"B{l}").Value.ToString(),
                    Movimentacao = planilha.Cell($"C{l}").Value.ToString(),
                    Produto = planilha.Cell($"D{l}").Value.ToString(),
                    Instituicao = planilha.Cell($"E{l}").Value.ToString(),
                    Quantidade = planilha.Cell($"F{l}").Value.ToString(),
                    PrecoUnitario = planilha.Cell($"G{l}").Value.ToString(),
                    ValorOperacao = planilha.Cell($"H{l}").Value.ToString()
                });
            }

            return lst;
        }

        private async Task<List<ExcelB3>> RetornaListaDividendosExcelB3(IXLWorksheet planilha)
        {
            List<ExcelB3> lst = new List<ExcelB3>();

            var totalLinhas = planilha.Rows().Count();

            for (int l = 2; l <= totalLinhas; l++)
            {
                lst.Add(new ExcelB3
                {
                    EntradaSaida = "Credito\r\n",
                    Data = planilha.Cell($"D{l}").Value.ToString(),
                    Movimentacao = planilha.Cell($"C{l}").Value.ToString(),
                    Produto = planilha.Cell($"A{l}").Value.ToString(),
                    Instituicao = planilha.Cell($"E{l}").Value.ToString(),
                    Quantidade = planilha.Cell($"G{l}").Value.ToString(),
                    PrecoUnitario = planilha.Cell($"H{l}").Value.ToString(),
                    ValorOperacao = planilha.Cell($"I{l}").Value.ToString()
                });
            }

            return lst;
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

            await _configuracaoService.ImportarExcelCotacaoAtual(fileCotacaoAtual, userId);

            return RedirectToAction("Index", "Papel");
        }

        public async Task<ActionResult> DeletarBanco()
        {
            _configuracaoService.DeletaTodoBanco(userId);

            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> ExportarBackup()
        {
            var stream = await ExportarProdutosParaExcel();

            return File(stream,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Ações.xlsx");
        }

        private async Task<MemoryStream> ExportarProdutosParaExcel()
        {
            try
            {
                List<PapelViewModel> lstP = _mapper.Map<List<PapelViewModel>>(await _papelService.Get(x => x.LoginId.ToString() == userId));
                List<DividendoViewModel> lstD = _mapper.Map<List<DividendoViewModel>>(await _dividendoService.Get(x => x.Papel.LoginId.ToString() == userId));
                List<TransacaoViewModel> lstT = _mapper.Map<List<TransacaoViewModel>>(await _transacaoService.Get(x => x.Papel.LoginId.ToString() == userId));

                var workbook = new XLWorkbook();

                #region P A P E L

                var worksheetPapel = workbook.Worksheets.Add("Papel");

                // Cabeçalhos
                worksheetPapel.Cell(1, 1).Value = "Id";
                worksheetPapel.Cell(1, 2).Value = "LoginId";
                worksheetPapel.Cell(1, 3).Value = "Código";
                worksheetPapel.Cell(1, 4).Value = "Nome";
                worksheetPapel.Cell(1, 5).Value = "Tipo";
                worksheetPapel.Cell(1, 6).Value = "Cot. atual";
                worksheetPapel.Cell(1, 7).Value = "Descrição";
                worksheetPapel.Cell(1, 8).Value = "Ativo";

                // Conteúdo
                for (int i = 0; i < lstP.Count; i++)
                {
                    var p = lstP.ToList().OrderBy(x => x.TipoPapel).OrderBy(x => x.Codigo).ToList()[i];
                    worksheetPapel.Cell(i + 2, 1).Value = p.Id.ToString();
                    worksheetPapel.Cell(i + 2, 2).Value = p.LoginId.ToString();
                    worksheetPapel.Cell(i + 2, 3).Value = p.Codigo;
                    worksheetPapel.Cell(i + 2, 4).Value = p.Nome;
                    worksheetPapel.Cell(i + 2, 5).Value = p.TipoPapel.ToString();
                    worksheetPapel.Cell(i + 2, 6).Value = p.CotacaoAtual;
                    worksheetPapel.Cell(i + 2, 7).Value = p.Descricao;
                    worksheetPapel.Cell(i + 2, 8).Value = p.Ativo;
                }

                // Ajustar colunas automaticamente
                worksheetPapel.Columns().AdjustToContents();

                #endregion

                #region T R S A Ç Ã O

                var worksheetTransacao = workbook.Worksheets.Add("Transação");

                // Cabeçalhos
                worksheetTransacao.Cell(1, 1).Value = "Id";
                worksheetTransacao.Cell(1, 2).Value = "Papel";
                worksheetTransacao.Cell(1, 3).Value = "Valor Unitário";
                worksheetTransacao.Cell(1, 4).Value = "Quantidade";
                worksheetTransacao.Cell(1, 5).Value = "Data";
                worksheetTransacao.Cell(1, 6).Value = "Tipo";
                worksheetTransacao.Cell(1, 7).Value = "Descrição";
                worksheetTransacao.Cell(1, 8).Value = "Ativo";

                // Conteúdo
                for (int i = 0; i < lstT.Count; i++)
                {
                    var p = lstT.ToList().OrderBy(x => x.Data).ToList()[i];
                    worksheetTransacao.Cell(i + 2, 1).Value = p.Id.ToString();
                    worksheetTransacao.Cell(i + 2, 2).Value = p.PapelId.ToString();
                    worksheetTransacao.Cell(i + 2, 3).Value = p.ValorUnt;
                    worksheetTransacao.Cell(i + 2, 4).Value = p.Quantidade;
                    worksheetTransacao.Cell(i + 2, 5).Value = p.Data;
                    worksheetTransacao.Cell(i + 2, 6).Value = p.TipoTransacao.ToString();
                    worksheetTransacao.Cell(i + 2, 7).Value = p.Descricao;
                    worksheetTransacao.Cell(i + 2, 8).Value = p.Ativo;
                }

                // Ajustar colunas automaticamente
                worksheetTransacao.Columns().AdjustToContents();

                #endregion

                #region D I V I D E N D O

                var worksheetDividendo = workbook.Worksheets.Add("Dividendo");

                // Cabeçalhos
                worksheetDividendo.Cell(1, 1).Value = "Id";
                worksheetDividendo.Cell(1, 2).Value = "Papel";
                worksheetDividendo.Cell(1, 3).Value = "Valor Recebido";
                worksheetDividendo.Cell(1, 4).Value = "Qtd";
                worksheetDividendo.Cell(1, 5).Value = "Data";
                worksheetDividendo.Cell(1, 6).Value = "Descrição";
                worksheetDividendo.Cell(1, 7).Value = "Ativo";
                worksheetDividendo.Cell(1, 8).Value = "TipoDividendo";
                //worksheetDividendo.Cell(1, 9).Value = "Historico";

                // Conteúdo
                for (int i = 0; i < lstD.Count; i++)
                {
                    var p = lstD.ToList().OrderBy(x => x.Data).ToList()[i];
                    worksheetDividendo.Cell(i + 2, 1).Value = p.Id.ToString();
                    worksheetDividendo.Cell(i + 2, 2).Value = p.PapelId.ToString();
                    worksheetDividendo.Cell(i + 2, 3).Value = p.ValorRecebido;
                    worksheetDividendo.Cell(i + 2, 4).Value = p.Quantidade;
                    worksheetDividendo.Cell(i + 2, 5).Value = p.Data;
                    worksheetDividendo.Cell(i + 2, 6).Value = p.Descricao;
                    worksheetDividendo.Cell(i + 2, 7).Value = p.Ativo;
                    worksheetDividendo.Cell(i + 2, 8).Value = p.TipoDividendo.ToString();
                    //worksheetDividendo.Cell(i + 2, 9).Value = p.Historico;
                }

                // Ajustar colunas automaticamente
                worksheetDividendo.Columns().AdjustToContents();

                #endregion

                // Salvar em memória
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;
                return stream;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<ActionResult> RestaurarBackup(HttpPostedFileBase fileBackup)
        {
            // Verifica se o arquivo é nulo
            if (fileBackup == null || fileBackup.ContentLength == 0)
            {
                throw new ArgumentException("O arquivo é nulo ou vazio.");
            }

            // Verifica se o arquivo é do tipo Excel
            if (!fileBackup.ContentType.Equals("application/vnd.ms-excel") &&
                !fileBackup.ContentType.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
            {
                throw new ArgumentException("O arquivo não é do tipo Excel.");
            }

            List<Papel> lstPapel = RetornaListas(fileBackup);

            foreach (var papel in lstPapel)
            {
                await _papelService.Add(papel);
            }

            return RedirectToAction("Index", "Papel");
        }

        private List<Papel> RetornaListas(HttpPostedFileBase fileB3)
        {
            List<Papel> lstPapel = new List<Papel>();
            using (var stream = fileB3.InputStream)
            {
                using (var workbook = new XLWorkbook(stream))
                {
                    var planilha = workbook.Worksheets.ToList();

                    #region P A P E L

                    var planilhaPapel = planilha.FirstOrDefault(x => x.Name == "Papel");
                    var totalLinhas = planilhaPapel.Rows().Count();
                    for (int l = 2; l <= totalLinhas; l++)
                    {
                        Papel p = new Papel();
                        p.Id = new Guid(planilhaPapel.Cell($"A{l}").Value.ToString());
                        p.LoginId = new Guid(planilhaPapel.Cell($"B{l}").Value.ToString());
                        p.Codigo = planilhaPapel.Cell($"C{l}").Value.ToString();
                        p.Nome = planilhaPapel.Cell($"D{l}").Value.ToString();
                        var tipoPapel = planilhaPapel.Cell($"E{l}").Value.ToString();
                        p.TipoPapel = tipoPapel == "Acao" ? TipoPapel.Acao : tipoPapel == "FII" ? TipoPapel.FII : tipoPapel == "BDR" ? TipoPapel.BDR : TipoPapel.ETF;
                        p.CotacaoAtual = Convert.ToDouble(planilhaPapel.Cell($"F{l}").Value.ToString());
                        p.Descricao = planilhaPapel.Cell($"G{l}").Value.ToString();
                        p.Ativo = planilhaPapel.Cell($"H{l}").Value.ToString() == "TRUE" || planilhaPapel.Cell($"H{l}").Value.ToString() == "VERDADEIRO";

                        lstPapel.Add(p);
                    }

                    #endregion

                    #region T R A S A Ç Ã O

                    var planilhaTransacao = planilha.FirstOrDefault(x => x.Name == "Transação");
                    totalLinhas = planilhaTransacao.Rows().Count();
                    for (int l = 2; l <= totalLinhas; l++)
                    {
                        Transacao t = new Transacao();
                        t.Id = new Guid(planilhaTransacao.Cell($"A{l}").Value.ToString());
                        t.PapelId = new Guid(planilhaTransacao.Cell($"B{l}").Value.ToString());
                        t.ValorUnt = Convert.ToDouble(planilhaTransacao.Cell($"C{l}").Value.ToString());
                        t.Quantidade = Convert.ToDouble(planilhaTransacao.Cell($"D{l}").Value.ToString());
                        t.Data = Convert.ToDateTime(planilhaTransacao.Cell($"E{l}").Value.ToString());
                        var tipoTransacao = planilhaTransacao.Cell($"F{l}").Value.ToString();
                        t.TipoTransacao = tipoTransacao == "Compra" ? TipoTransacao.Compra : TipoTransacao.Venda;
                        t.Descricao = planilhaTransacao.Cell($"G{l}").Value.ToString();
                        t.Ativo = planilhaTransacao.Cell($"H{l}").Value.ToString() == "TRUE" || planilhaTransacao.Cell($"H{l}").Value.ToString() == "VERDADEIRO";

                        if (lstPapel.Where(x => x.Id == t.PapelId).FirstOrDefault().Transacao == null)
                            lstPapel.Where(x => x.Id == t.PapelId).FirstOrDefault().Transacao = new List<Transacao>();

                        lstPapel.Where(x => x.Id == t.PapelId).FirstOrDefault().Transacao.Add(t);
                        //lstTransacao.Add(t);
                    }

                    #endregion

                    #region D I V I D E N D O

                    var planilhaDividendo = planilha.FirstOrDefault(x => x.Name == "Dividendo");
                    totalLinhas = planilhaDividendo.Rows().Count();
                    for (int l = 2; l <= totalLinhas; l++)
                    {
                        Dividendo d = new Dividendo();
                        d.Id = new Guid(planilhaDividendo.Cell($"A{l}").Value.ToString());
                        d.PapelId = new Guid(planilhaDividendo.Cell($"B{l}").Value.ToString());
                        d.ValorRecebido = Convert.ToDouble(planilhaDividendo.Cell($"C{l}").Value.ToString());
                        d.Quantidade = Convert.ToDouble(planilhaDividendo.Cell($"D{l}").Value.ToString());
                        d.Data = Convert.ToDateTime(planilhaDividendo.Cell($"E{l}").Value.ToString());
                        d.Descricao = planilhaDividendo.Cell($"F{l}").Value.ToString();
                        d.Ativo = planilhaDividendo.Cell($"G{l}").Value.ToString() == "TRUE" || planilhaDividendo.Cell($"G{l}").Value.ToString() == "VERDADEIRO" ? true : false;
                        var tipoDividendo = planilhaDividendo.Cell($"H{l}").Value.ToString();
                        d.TipoDividendo = tipoDividendo == "JSCP" ? TipoDividendo.JSCP : tipoDividendo == "Rendimento" ? TipoDividendo.Rendimento : tipoDividendo == "Dividendo" ? TipoDividendo.Dividendo : TipoDividendo.Outro;
                        //d.Historico = planilhaDividendo.Cell($"I{l}").Value.ToString();

                        if (lstPapel.Where(x => x.Id == d.PapelId).FirstOrDefault().Dividendo == null)
                            lstPapel.Where(x => x.Id == d.PapelId).FirstOrDefault().Dividendo = new List<Dividendo>();

                        lstPapel.Where(x => x.Id == d.PapelId).FirstOrDefault().Dividendo.Add(d);
                        //lstDividendo.Add(d);
                    }
                    #endregion

                }
            }

            return lstPapel;
        }

    }
}