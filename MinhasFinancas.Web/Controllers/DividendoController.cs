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
    public class DividendoController : BaseController
    {
        IDividendoService _dividendoService;
        IPapelService _papelService;
        ITransacaoService _transacaoService;
        IMapper _mapper;

        public DividendoController(IDividendoService dividendoService,
                                    IPapelService papelService,
                                    ITransacaoService transacaoService,
                                    IMapper mapper,
                                    INotificador notificador) : base(notificador)
        {
            _dividendoService = dividendoService;
            _papelService = papelService;
            _transacaoService = transacaoService;
            _mapper = mapper;
        }

        // GET: Dividendo
        public async Task<ActionResult> Index(int cbbTipoPapel = 0, string dtInicio = null, string dtFim = null)
        {
            List<DividendoViewModel> lst = _mapper.Map<List<DividendoViewModel>>(await _dividendoService.Get(includeProperties: "Papel")).ToList();

            if (cbbTipoPapel != 0)
                lst = lst.Where(f => Convert.ToInt32(f.Papel.TipoPapel) == cbbTipoPapel).ToList();
            if (!string.IsNullOrEmpty(dtInicio) && !string.IsNullOrEmpty(dtFim))
                lst = lst.Where(f => f.Data >= Convert.ToDateTime(dtInicio) && f.Data <= Convert.ToDateTime(dtFim)).ToList();

            ViewBag.dividendoTotal = lst.Sum(f => f.ValorRecebido);

            List<TransacaoViewModel> lstTransacao = _mapper.Map<List<TransacaoViewModel>>(await _transacaoService.Get());

            lst.OrderBy(s => s.Data).ToList().ForEach(f => {

                var lstT2 = new List<TransacaoViewModel>();

                foreach (var t in lstTransacao.Where(a => a.PapelId == f.PapelId).OrderBy(s => s.Data).ToList())
                {
                    if (lstT2.Sum(y => y.Quantidade) < f.Quantidade)
                        lstT2.Add(t);
                    else
                        break;
                }

                f.PrecoMedio = lstT2.Sum(y => y.Quantidade * y.ValorUnt) / f.Quantidade;
                
                f.YieldOnCost = f.ValorRecebido / (f.PrecoMedio * f.Quantidade) * 100;
            });

            return View(lst.OrderBy(f => f.Data));
        }

        // GET: Dividendo/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(Guid id)
        {
            DividendoViewModel dividendoViewModel = _mapper.Map<DividendoViewModel>(await _dividendoService.GetById(id));
            if (dividendoViewModel == null)
            {
                return HttpNotFound();
            }

            dividendoViewModel.Papel = _mapper.Map<List<PapelViewModel>>(await _papelService.Get(f => f.Id == dividendoViewModel.PapelId)).FirstOrDefault();
            ViewBag.Papel = dividendoViewModel.Papel.Codigo + " - " + dividendoViewModel.Papel.Nome + " | " + dividendoViewModel.Data.ToString("dd/MM/yyyy");
            return View(dividendoViewModel);
        }

        // GET: Dividendo/Create
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var dividendoViewModel = await PopularPapeis(new DividendoViewModel());

            return View(dividendoViewModel);
        }

        // POST: Dividendo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DividendoViewModel dividendoViewModel)
        {
            if (!ModelState.IsValid) return View(dividendoViewModel);

            Dividendo obj = _mapper.Map<Dividendo>(dividendoViewModel);
            await _dividendoService.Add(obj);

            if (!OperacaoValida()) return View(dividendoViewModel);

            return RedirectToAction("Index");
        }

        // GET: Dividendo/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            DividendoViewModel dividendoViewModel = _mapper.Map<DividendoViewModel>(await _dividendoService.GetById(id));
            if (dividendoViewModel == null)
            {
                return HttpNotFound();
            }

            dividendoViewModel.Papel = _mapper.Map<List<PapelViewModel>>(await _papelService.Get(f => f.Id == dividendoViewModel.PapelId)).FirstOrDefault();
            dividendoViewModel = await PopularPapeis(dividendoViewModel);
            ViewBag.Papel = dividendoViewModel.Papel.Codigo + " - " + dividendoViewModel.Papel.Nome + " | " + dividendoViewModel.Data.ToString("dd/MM/yyyy");

            return View(dividendoViewModel);
        }

        // POST: Dividendo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, DividendoViewModel dividendoViewModel)
        {
            if (id != dividendoViewModel.Id) return HttpNotFound();

            if (!ModelState.IsValid) return View(dividendoViewModel);

            Dividendo dividendo = _mapper.Map<Dividendo>(dividendoViewModel);
            await _dividendoService.Update(dividendo);

            if (!OperacaoValida()) return View(_mapper.Map<DividendoViewModel>(await _dividendoService.GetById(id)));

            return RedirectToAction("Index");
        }

        // GET: Dividendo/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            DividendoViewModel dividendoViewModel = _mapper.Map<DividendoViewModel>(await _dividendoService.GetById(id));
            if (dividendoViewModel == null)
            {
                return HttpNotFound();
            }

            dividendoViewModel.Papel = _mapper.Map<List<PapelViewModel>>(await _papelService.Get(f => f.Id == dividendoViewModel.PapelId)).FirstOrDefault();
            ViewBag.Papel = dividendoViewModel.Papel.Codigo + " - " + dividendoViewModel.Papel.Nome + " | " + dividendoViewModel.Data.ToString("dd/MM/yyyy");

            return View(dividendoViewModel);
        }

        // POST: Dividendo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            DividendoViewModel dividendoViewModel = _mapper.Map<DividendoViewModel>(await _dividendoService.GetById(id));

            if (dividendoViewModel == null) return HttpNotFound();

            await _dividendoService.DeleteById(id);

            if (!OperacaoValida()) return View(dividendoViewModel);

            return RedirectToAction("Index");
        }


        private async Task<DividendoViewModel> PopularPapeis(DividendoViewModel dividendo)
        {
            dividendo.Papels = _mapper.Map<List<PapelViewModel>>(await _papelService.Get());
            return dividendo;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dividendoService.Dispose();
                _papelService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
