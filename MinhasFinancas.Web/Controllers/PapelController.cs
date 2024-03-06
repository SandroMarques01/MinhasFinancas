using AutoMapper;
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
            List<PapelViewModel> lst = _mapper.Map<List<PapelViewModel>>(await _papelService.Get(includeProperties: "Transacao,Dividendo"));

            //if (!string.IsNullOrEmpty(dtFim))
            //    lst = lst.Where(f => f.Transacao.ToList().Data <= Convert.ToDateTime(dtFim)).ToList();

            if (cbbTipoPapel != 0)
                lst = lst.Where(f => Convert.ToInt32(f.TipoPapel) == cbbTipoPapel).ToList();

            double totalSaldo = 0;
            double totalSaldoAtual = 0;
            double totalDividendos = 0;
            lst.ForEach(f =>
            {
                //Calculo de preços
                f.QuantidadeTotal = f.Transacao.Sum(x => x.TipoTransacao == Infra.TipoTransacao.Compra ? x.Quantidade : x.Quantidade * -1);

                f.PrecoMedio = f.QuantidadeTotal == 0 ? 0 : f.Transacao.Sum(x => x.Quantidade * x.ValorUnt) / f.QuantidadeTotal;

                f.TotalSaldo = f.QuantidadeTotal == 0 ? 0 : f.Transacao.Sum(x => x.Quantidade * x.ValorUnt);
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

        // GET: Papel/Details/5
        public async Task<ActionResult> Details(Guid id)
        {
            PapelViewModel papelViewModel = _mapper.Map<PapelViewModel>(await _papelService.GetById(id));
            if (papelViewModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.Papel = papelViewModel.Codigo + " - " + papelViewModel.Nome;

            return View(papelViewModel);
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
            List<PapelViewModel> lst = _mapper.Map<List<PapelViewModel>>(await _papelService.Get());

            TrocaPapelViewModel trocaPapelViewModel = new TrocaPapelViewModel();
            trocaPapelViewModel.PapelsOrigem = lst;
            trocaPapelViewModel.PapelsDestino = lst;

            return View(trocaPapelViewModel);
        }

        [HttpPost]

        public async Task<ActionResult> TrocaPapel(TrocaPapelViewModel trocaPapelViewModel)
        {
            await _papelService.TrocaPapel(trocaPapelViewModel.PapelIdOrigem, trocaPapelViewModel.PapelIdDestino);

            return View(Index());
        }
        

    }
}
