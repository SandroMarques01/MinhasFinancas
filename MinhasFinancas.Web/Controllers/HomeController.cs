using AutoMapper;
using MinhasFinancas.Infra.Models;
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
            DateTime ultimoDiaMes = Convert.ToDateTime(DateTime.Now.Year + "-" + (DateTime.Now.Month + 1) + "-01").AddDays(-1);


            List<DividendoViewModel> lstD = _mapper.Map<List<DividendoViewModel>>(await _dividendoService.Get(includeProperties: "Papel"));
            List<TransacaoViewModel> lstT = _mapper.Map<List<TransacaoViewModel>>(await _transacaoService.Get(includeProperties: "Papel"));
            lstT = lstT.ToList().Where(x => x.Papel.Ativo).ToList();

            double totalInvestido = lstT.Where(x=>x.TipoTransacao == Infra.TipoTransacao.Compra).Sum(x => x.Quantidade * x.ValorUnt);

            //dividendos
            ViewBag.DividendosMes = lstD.Where(f => f.Data >= Convert.ToDateTime(DateTime.Now.Year + "-" + (DateTime.Now.Month) + "-01")
                                                    && f.Data <= ultimoDiaMes).Sum(x => x.ValorRecebido);

            ViewBag.DividendosFiisMes = lstD.Where(f => f.Papel.TipoPapel == Infra.TipoPapel.Fii && f.Data >= Convert.ToDateTime(DateTime.Now.Year + "-" + (DateTime.Now.Month) + "-01")
                                                    && f.Data <= ultimoDiaMes).Sum(x => x.ValorRecebido);
            ViewBag.DividendosFiisMesPercent = ViewBag.DividendosFiisMes*100 / lstT.Where(x => x.Papel.TipoPapel == Infra.TipoPapel.Fii
                                                                            && x.TipoTransacao == Infra.TipoTransacao.Compra).Sum(x => x.Quantidade * x.ValorUnt);

            ViewBag.ValorAtualCarteira = totalInvestido;

            lstD = lstD.Where(f => f.Data >= DateTime.Now.AddDays(-1) && f.Data <= DateTime.Now.AddDays(6)).ToList();

            ViewBag.ValorDividendoSemana = lstD.Sum(x => x.ValorRecebido);
            ViewBag.TabelaDividendosSemana = lstD.OrderBy(x => x.Data);

            //transação
            lstT = lstT.Where(f => f.Data >= DateTime.Now.AddDays(-2) && f.Data <= DateTime.Now.AddDays(10)).ToList();

            ViewBag.ValorTransacaoSemana = lstT.Sum(x => (Convert.ToInt32(x.TipoTransacao) == 1 ? x.ValorUnt : x.ValorUnt * -1) * x.Quantidade);
            ViewBag.TabelaUltimasTransacoes = lstT.OrderBy(x => x.Data);

            List<PapelViewModel> lstP = _mapper.Map<List<PapelViewModel>>(await _papelService.Get(includeProperties: "Transacao,Dividendo"));
            lstP.ForEach(f =>
            {
                f.QuantidadeTotal = f.Transacao.Sum(x => x.TipoTransacao == Infra.TipoTransacao.Compra ? x.Quantidade : x.Quantidade * -1);
                f.TotalSaldoAtual = f.CotacaoAtual * f.QuantidadeTotal;
                f.DividendosTotal = f.Dividendo.Sum(x => x.ValorRecebido);
            });

            ViewBag.ValorTotalPapel = lstP.Sum(x => x.TotalSaldoAtual);
            ViewBag.TabelaMaioresPapel = lstP.OrderByDescending(x => x.TotalSaldoAtual).Take(10);


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