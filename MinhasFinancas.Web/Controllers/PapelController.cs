using AutoMapper;
using MinhasFinancas.Infra;
using MinhasFinancas.Infra.Models;
using MinhasFinancas.Service.Core;
using MinhasFinancas.Service.Papel;
using MinhasFinancas.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MinhasFinancas.Web.Controllers
{

    public class PapelController : BaseController
    {
        IPapelService _papelService;
        IMapper _mapper;

        public PapelController(IPapelService papelService,
                               IMapper mapper,
                               INotificador notificador) : base(notificador)
        {
            _papelService = papelService;
            _mapper = mapper;
        }

        // GET: Papel
        public async Task<ActionResult> Index(int cbbTipoPapel = 0, bool Ativo = true, string dtFim = null)
        {
            if (string.IsNullOrEmpty(userId))
                return Redirect(@"/Login/Index");

            List<PapelViewModel> lst = _mapper.Map<List<PapelViewModel>>(await _papelService.Get(x => x.LoginId.ToString() == userId, includeProperties: "Transacao,Dividendo"));

            if (cbbTipoPapel != 0)
                lst = lst.Where(f => Convert.ToInt32(f.TipoPapel) == cbbTipoPapel).ToList();

            ViewBag.DataFim = dtFim;

            if (!string.IsNullOrEmpty(dtFim))
            {
                foreach (var item in lst)
                {
                    item.Transacao = item.Transacao.Where(x => x.Data <= Convert.ToDateTime(dtFim)).ToList();
                    item.Dividendo = item.Dividendo.Where(x => x.Data <= Convert.ToDateTime(dtFim)).ToList();
                }
            }


            double totalSaldo = 0;
            double totalSaldoAtual = 0;
            double totalDividendos = 0;
            lst.ForEach(f =>
            {
                //Calculo de preços
                f.QuantidadeTotal = f.Transacao.Sum(x => x.TipoTransacao == TipoTransacao.Compra ? x.Quantidade : x.Quantidade * -1);

                f.PrecoMedio = CalculoPrecoMedio(f.Transacao.ToList());

                f.TotalSaldo = f.QuantidadeTotal * f.PrecoMedio;
                totalSaldo += f.TotalSaldo;

                f.TotalSaldoAtual = f.CotacaoAtual * f.QuantidadeTotal;
                totalSaldoAtual += f.TotalSaldoAtual;

                f.Valorizacao = Math.Round(((f.TotalSaldoAtual / f.TotalSaldo) * 100) - 100, 2);
                f.GanhoUnt = Math.Round(f.CotacaoAtual - f.PrecoMedio, 2);
                f.GanhoTotal = (f.CotacaoAtual - f.PrecoMedio) * f.QuantidadeTotal;

                // Dividendos

                f.DividendosTotal = f.Dividendo.Sum(x => x.ValorRecebido);
                f.PercentDividendos = Math.Round((f.DividendosTotal / f.TotalSaldo) * 100, 2);
                totalDividendos += f.DividendosTotal;

                // 2 casas decimais
                f.PrecoMedio = Math.Round(f.PrecoMedio, 2);
                f.TotalSaldo = Math.Round(f.TotalSaldo, 2);
                f.TotalSaldoAtual = Math.Round(f.TotalSaldoAtual, 2);
                f.DividendosTotal = Math.Round(f.DividendosTotal, 2);
            });

            ViewBag.totalSaldo = Math.Round(totalSaldo, 2);
            ViewBag.totalSaldoAtual = Math.Round(totalSaldoAtual, 2);
            ViewBag.DYtotal = Math.Round(((totalDividendos / totalSaldo) * 100), 2);

            if (Ativo)
                lst = lst.Where(x => x.QuantidadeTotal > 0).ToList();
            else
                lst = lst.Where(x => x.QuantidadeTotal == 0).ToList();


            return View(lst.OrderBy(f => f.TipoPapel).OrderByDescending(f => f.TotalSaldo));
        }

        //private double CalculoPrecoMedio(List<TransacaoViewModel> transacoes)
        //{
        //    double precoMedio = 0;
        //    double valorYotal = 0;
        //    double qtdYotal = 0;
        //    foreach (var transacao in transacoes.OrderBy(o => o.Data))
        //    {
        //        if (transacao.TipoTransacao == TipoTransacao.Compra)
        //        {
        //            valorYotal += transacao.Quantidade * transacao.ValorUnt;
        //            qtdYotal += transacao.Quantidade;
        //            precoMedio = valorYotal / qtdYotal;
        //        }
        //        else
        //        {
        //            qtdYotal -= transacao.Quantidade;
        //            valorYotal = precoMedio * qtdYotal;
        //        }
        //    }

        //    return precoMedio;
        //}

        [HttpGet]
        public async Task<ActionResult> Detalhes(string codigo)
        {
            PapelViewModel papelViewModel;
            try
            {
                var listpapelViewModel = _mapper.Map<List<PapelViewModel>>(await _papelService.Get(x => x.Codigo == codigo && x.LoginId.ToString() == userId, "Transacao,Dividendo"));
                papelViewModel = listpapelViewModel.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return View("Código não encontrado!");
            }

            if (papelViewModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.Papel = papelViewModel.Codigo + " - " + papelViewModel.Nome;


            double evolucaoDiv = 0;
            papelViewModel.Dividendo.OrderBy(s => s.Data).ToList().ForEach(f =>
            {

                evolucaoDiv += f.ValorRecebido;
                f.Evolucao = evolucaoDiv;

                var lstT2 = new List<TransacaoViewModel>();

                foreach (var t in papelViewModel.Transacao.Where(a => a.PapelId == f.PapelId).OrderBy(s => s.Data).ToList())
                {
                    if (lstT2.Sum(y => y.Quantidade) < f.Quantidade)
                        lstT2.Add(t);
                    else
                        break;
                }

                f.PrecoMedio = lstT2.Sum(y => y.Quantidade * y.ValorUnt) / f.Quantidade;

                f.YieldOnCost = f.ValorRecebido / (f.PrecoMedio * f.Quantidade) * 100;
            });

            double evolucao = 0;
            double evolucaoAtual = 0;
            papelViewModel.Transacao.OrderBy(s => s.Data).ToList().ForEach(f =>
            {
                evolucao += f.Quantidade * f.ValorUnt;
                f.Evolucao = evolucao;

                evolucaoAtual += f.Quantidade * papelViewModel.CotacaoAtual;
                f.EvolucaoAtual = evolucaoAtual;

            });

            ViewBag.TabelaTransacaoPorPapel = papelViewModel.Transacao.OrderBy(x => x.Data);
            ViewBag.TabelaDividendosPorPapel = papelViewModel.Dividendo.OrderBy(x => x.Data);

            return View(papelViewModel);
        }

        //GET: Papel/Details/5
        public async Task<ActionResult> Details(Guid id)
        {
            PapelViewModel papelViewModel;
            try
            {
                var listpapelViewModel = _mapper.Map<List<PapelViewModel>>(await _papelService.Get(x => x.Id == id && x.LoginId.ToString() == userId, "Transacao,Dividendo"));
                papelViewModel = listpapelViewModel.FirstOrDefault();

                return Redirect(@"/" + papelViewModel.TipoPapel.ToString() + "/" + papelViewModel.Codigo);
            }
            catch (Exception ex)
            {
                return View("");
                throw ex;
            }

        }

        // GET: Papel/Create
        public async Task<ActionResult> Create()
        {
            return View();
        }

        // POST: Papel/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PapelViewModel papelViewModel)
        {
            if (!ModelState.IsValid) return View(papelViewModel);

            Papel obj = _mapper.Map<Papel>(papelViewModel);
            obj.LoginId = new Guid(userId);
            await _papelService.Add(obj);

            if (!OperacaoValida()) return View(papelViewModel);

            return RedirectToAction("Index");
        }

        // GET: Papel/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            PapelViewModel papelViewModel = _mapper.Map<PapelViewModel>(await _papelService.GetById(id));
            if (papelViewModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.Papel = papelViewModel.Codigo + " - " + papelViewModel.Nome;

            return View(papelViewModel);
        }

        // POST: Papel/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, PapelViewModel papelViewModel)
        {
            if (id != papelViewModel.Id) return HttpNotFound();

            if (!ModelState.IsValid) return View(papelViewModel);

            Papel papel = _mapper.Map<Papel>(papelViewModel);
            await _papelService.Update(papel);

            if (!OperacaoValida()) return View(_mapper.Map<PapelViewModel>(await _papelService.GetById(id)));

            return RedirectToAction("Index");
        }

        // GET: Papel/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            PapelViewModel papelViewModel = _mapper.Map<PapelViewModel>(await _papelService.GetById(id));
            if (papelViewModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.Papel = papelViewModel.Codigo + " - " + papelViewModel.Nome;

            return View(papelViewModel);
        }

        // POST: Papel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            PapelViewModel papelViewModel = _mapper.Map<PapelViewModel>(await _papelService.GetById(id));

            if (papelViewModel == null) return HttpNotFound();

            await _papelService.DeleteById(id);

            if (!OperacaoValida()) return View(papelViewModel);

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _papelService.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Papel/TrocaPapel/5
        public async Task<ActionResult> TrocaPapel()
        {
            List<PapelViewModel> lst = _mapper.Map<List<PapelViewModel>>(await _papelService.Get(x => x.LoginId.ToString() == userId));

            TrocaPapelViewModel trocaPapelViewModel = new TrocaPapelViewModel();
            trocaPapelViewModel.PapelsOrigem = lst.OrderBy(x => x.Codigo);
            trocaPapelViewModel.PapelsDestino = lst.OrderBy(x => x.Codigo);

            return View(trocaPapelViewModel);
        }

        [HttpPost]

        public async Task<ActionResult> TrocaPapel(TrocaPapelViewModel trocaPapelViewModel)
        {
            await _papelService.TrocaPapel(trocaPapelViewModel.PapelIdOrigem, trocaPapelViewModel.PapelIdDestino);

            return RedirectToAction("Index");
        }

        // GET: Papel/TrocaPapel/5
        public async Task<ActionResult> Desdobramento()
        {
            List<PapelViewModel> lst = _mapper.Map<List<PapelViewModel>>(await _papelService.Get(x => x.LoginId.ToString() == userId));

            TrocaPapelViewModel trocaPapelViewModel = new TrocaPapelViewModel();
            trocaPapelViewModel.PapelsOrigem = lst.OrderBy(x => x.Codigo);

            return View(trocaPapelViewModel);
        }

        [HttpPost]

        public async Task<ActionResult> Desdobramento(TrocaPapelViewModel trocaPapelViewModel)
        {
            await _papelService.Desdobramento(trocaPapelViewModel.PapelIdOrigem, trocaPapelViewModel.QuantidadeDesdobro, trocaPapelViewModel.DataDesdobro, userId);

            return RedirectToAction("Index");
        }
    }
}
