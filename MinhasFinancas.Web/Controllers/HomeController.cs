using AutoMapper;
using MinhasFinancas.Service.Core;
using MinhasFinancas.Service.Dividendo;
using MinhasFinancas.Service.Papel;
using MinhasFinancas.Service.Transacao;
using MinhasFinancas.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MinhasFinancas.Web.Controllers
{
    public class HomeController : BaseController
    {
        IPapelService _papelService;
        IDividendoService _dividendoService;
        ITransacaoService _transacaoService;
        IMapper _mapper;

        public HomeController(IPapelService papelService,
                                IDividendoService dividendoService,
                                ITransacaoService transacaoService,
                                IMapper mapper,
                                INotificador notificador) : base(notificador)
        {
            _papelService = papelService;
            _dividendoService = dividendoService;
            _transacaoService = transacaoService;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index()
        {
            if (string.IsNullOrEmpty(userId))
                return Redirect(@"/Login/Index");

            DateTime ultimoDiaMes = Convert.ToDateTime((DateTime.Now.Month == 12 ? DateTime.Now.Year + 1 : DateTime.Now.Year)
                                                        + "-" + 
                                                        (DateTime.Now.Month == 12 ? "01" : (DateTime.Now.Month + 1).ToString()) 
                                                        + "-01").AddDays(-1);

            List<DividendoViewModel> lstD = _mapper.Map<List<DividendoViewModel>>(await _dividendoService.Get(x => x.Papel.LoginId.ToString() == userId, includeProperties: "Papel"));
            List<TransacaoViewModel> lstT = _mapper.Map<List<TransacaoViewModel>>(await _transacaoService.Get(x => x.Papel.LoginId.ToString() == userId, includeProperties: "Papel"));
            lstT = lstT.ToList().Where(x => x.Papel.Ativo).ToList();

            double totalInvestido = lstT.Where(x => x.TipoTransacao == Infra.TipoTransacao.Compra).Sum(x => x.Quantidade * x.ValorUnt);

            //dividendos
            ViewBag.DividendosMes = lstD.Where(f => f.Data >= Convert.ToDateTime(DateTime.Now.Year + "-" + (DateTime.Now.Month) + "-01")
                                                    && f.Data <= ultimoDiaMes).Sum(x => x.ValorRecebido);

            ViewBag.DividendosFiisMes = lstD.Where(f => f.Papel.TipoPapel == Infra.TipoPapel.FII && f.Data >= Convert.ToDateTime(DateTime.Now.Year + "-" + (DateTime.Now.Month) + "-01")
                                                    && f.Data <= ultimoDiaMes).Sum(x => x.ValorRecebido);

            ViewBag.DividendosFiisMesPercent = ViewBag.DividendosFiisMes * 100 / lstT.Where(x => x.Papel.TipoPapel == Infra.TipoPapel.FII
                                                                            && x.TipoTransacao == Infra.TipoTransacao.Compra
                                                                            && x.Data < Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-01")).Sum(x => x.Quantidade * x.ValorUnt);
            
            ViewBag.ValorAtualCarteira = totalInvestido;

            lstD = lstD.Where(f => f.Data >= DateTime.Now.AddDays(-1) && f.Data <= DateTime.Now.AddDays(6)).ToList();

            ViewBag.ValorDividendoSemana = lstD.Sum(x => x.ValorRecebido);
            ViewBag.TabelaDividendosSemana = lstD.OrderBy(x => x.Data);

            //transação
            lstT = lstT.Where(f => f.Data >= DateTime.Now.AddDays(-2) && f.Data <= DateTime.Now.AddDays(10)).ToList();

            ViewBag.ValorTransacaoSemana = lstT.Sum(x => (Convert.ToInt32(x.TipoTransacao) == 1 ? x.ValorUnt : x.ValorUnt * -1) * x.Quantidade);
            ViewBag.TabelaUltimasTransacoes = lstT.OrderBy(x => x.Data);

            List<PapelViewModel> lstP = _mapper.Map<List<PapelViewModel>>(await _papelService.Get(x => x.LoginId.ToString() == userId, includeProperties: "Transacao,Dividendo"));
            lstP.ForEach(f =>
            {
                f.QuantidadeTotal = f.Transacao.Sum(x => x.TipoTransacao == Infra.TipoTransacao.Compra ? x.Quantidade : x.Quantidade * -1);
                f.TotalSaldoAtual = f.CotacaoAtual * f.QuantidadeTotal;
                f.DividendosTotal = f.Dividendo.Sum(x => x.ValorRecebido);
            });

            ViewBag.ValorTotalPapel = lstP.Sum(x => x.TotalSaldoAtual);
            ViewBag.TabelaMaioresPapel = lstP.OrderByDescending(x => x.TotalSaldoAtual).Take(10);

            ViewBag.PercentAcao = lstP.Where(x => x.TipoPapel == Infra.TipoPapel.Acao).Sum(x => x.TotalSaldoAtual);
            ViewBag.PercentFII = lstP.Where(x => x.TipoPapel == Infra.TipoPapel.FII).Sum(x => x.TotalSaldoAtual);
            ViewBag.PercentBDR = lstP.Where(x => x.TipoPapel == Infra.TipoPapel.BDR).Sum(x => x.TotalSaldoAtual);
            ViewBag.PercentETF = lstP.Where(x => x.TipoPapel == Infra.TipoPapel.ETF).Sum(x => x.TotalSaldoAtual);

            //Gráfico Dividendos
            List<DividendoViewModel> lstDGraficoAcao = _mapper.Map<List<DividendoViewModel>>(await _dividendoService.RetornaTotalDividendosPorMes(userId, 5, tipoPapel: 1));
            ViewData["DivGraficoAcao"] = lstDGraficoAcao;
            List<DividendoViewModel> lstDGraficoFII = _mapper.Map<List<DividendoViewModel>>(await _dividendoService.RetornaTotalDividendosPorMes(userId, 5, tipoPapel: 2));
            ViewData["DivGraficoFII"] = lstDGraficoFII;
            List<DividendoViewModel> lstDGraficoBDR = _mapper.Map<List<DividendoViewModel>>(await _dividendoService.RetornaTotalDividendosPorMes(userId, 5, tipoPapel: 3));
            ViewData["DivGraficoBDR"] = lstDGraficoBDR;
            List<DividendoViewModel> lstDGraficoETF = _mapper.Map<List<DividendoViewModel>>(await _dividendoService.RetornaTotalDividendosPorMes(userId, 5, tipoPapel: 4));
            ViewData["DivGraficoETF"] = lstDGraficoETF;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}