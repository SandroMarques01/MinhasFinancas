﻿using AutoMapper;
using MinhasFinancas.Infra.Models;
using MinhasFinancas.Service.Core;
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
    public class TransacaoController : BaseController
    {
        ITransacaoService _transacaoService;
        IPapelService _papelService;
        IMapper _mapper;

        public TransacaoController(ITransacaoService transacaoService, 
                                    IPapelService papelService, 
                                    IMapper mapper,
                                    INotificador notificador) : base(notificador)
        {
            _transacaoService = transacaoService;
            _papelService = papelService;
            _mapper = mapper;
        }

        // GET: Transacao
        public async Task<ActionResult> Index()
        {
            return View(_mapper.Map<List<TransacaoViewModel>>(await _transacaoService.Get(includeProperties: "Papel")));
        }

        // GET: Transacao/Details/5
        public async Task<ActionResult> Details(Guid id)
        {
            TransacaoViewModel transacaoViewModel = _mapper.Map<TransacaoViewModel>(await _transacaoService.GetById(id));
            if (transacaoViewModel == null)
            {
                return HttpNotFound();
            }

            transacaoViewModel.Papel = _mapper.Map<List<PapelViewModel>>(await _papelService.Get(f => f.Id == transacaoViewModel.PapelId)).FirstOrDefault();

            return View(transacaoViewModel);
        }

        // GET: Transacao/Create
        public async Task<ActionResult> Create()
        {
            var transacaoViewModel = await PopularPapeis(new TransacaoViewModel());

            return View(transacaoViewModel);
        }

        // POST: Transacao/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TransacaoViewModel transacaoViewModel)
        {
            if (!ModelState.IsValid) return View(transacaoViewModel);

            Transacao obj = _mapper.Map<Transacao>(transacaoViewModel);
            await _transacaoService.Add(obj);

            if (!OperacaoValida()) return View(transacaoViewModel);

            return RedirectToAction("Index");
        }

        // GET: Transacao/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            TransacaoViewModel transacaoViewModel = _mapper.Map<TransacaoViewModel>(await _transacaoService.GetById(id));
            if (transacaoViewModel == null)
            {
                return HttpNotFound();
            }

            transacaoViewModel.Papel = _mapper.Map<List<PapelViewModel>>(await _papelService.Get(f => f.Id == transacaoViewModel.PapelId)).FirstOrDefault();
            transacaoViewModel = await PopularPapeis(transacaoViewModel);

            return View(transacaoViewModel);
        }

        // POST: Transacao/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, TransacaoViewModel transacaoViewModel)
        {
            if (id != transacaoViewModel.Id) return HttpNotFound();

            if (!ModelState.IsValid) return View(transacaoViewModel);

            Transacao transacao = _mapper.Map<Transacao>(transacaoViewModel);
            await _transacaoService.Update(transacao);

            if (!OperacaoValida()) return View(_mapper.Map<TransacaoViewModel>(await _transacaoService.GetById(id)));

            return RedirectToAction("Index");
        }

        // GET: Transacao/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            TransacaoViewModel transacaoViewModel = _mapper.Map<TransacaoViewModel>(await _transacaoService.GetById(id));
            if (transacaoViewModel == null)
            {
                return HttpNotFound();
            }

            transacaoViewModel.Papel = _mapper.Map<List<PapelViewModel>>(await _papelService.Get(f => f.Id == transacaoViewModel.PapelId)).FirstOrDefault();

            return View(transacaoViewModel);
        }

        // POST: Transacao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            TransacaoViewModel transacaoViewModel = _mapper.Map<TransacaoViewModel>(await _transacaoService.GetById(id));

            if (transacaoViewModel == null) return HttpNotFound();

            await _transacaoService.DeleteById(id);

            if (!OperacaoValida()) return View(transacaoViewModel);

            return RedirectToAction("Index");
        }


        private async Task<TransacaoViewModel> PopularPapeis(TransacaoViewModel transacao)
        {
            transacao.Papels = _mapper.Map<List<PapelViewModel>>(await _papelService.Get());
            return transacao;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _transacaoService.Dispose();
                _papelService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
