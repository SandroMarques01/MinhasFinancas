using AutoMapper;
using MinhasFinancas.Infra.Models;
using MinhasFinancas.Service.Ativo;
using MinhasFinancas.Service.Core;
using MinhasFinancas.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MinhasFinancas.Web.Controllers
{
    public class AtivoController : BaseController
    {
        IAtivoService _ativoService;
        IMapper _mapper;

        public AtivoController(IAtivoService ativoService,
                               IMapper mapper,
                               INotificador notificador) : base(notificador)
        {
            _ativoService = ativoService;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index()
        {
            if (string.IsNullOrEmpty(userId))
                return Redirect(@"/Login/Index");

            List<AtivoViewModel> lst = _mapper.Map<List<AtivoViewModel>>(await _ativoService.Get(x => x.LoginId.ToString() == userId));

            return View(lst);
        }

        public async Task<ActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AtivoViewModel ativoViewModel)
        {
            if (!ModelState.IsValid) return View(ativoViewModel);

            Ativo obj = _mapper.Map<Ativo>(ativoViewModel);
            obj.LoginId = new Guid(userId);
            await _ativoService.Add(obj);

            if (!OperacaoValida()) return View(ativoViewModel);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(Guid id)
        {
            AtivoViewModel ativoViewModel = _mapper.Map<AtivoViewModel>(await _ativoService.GetById(id));
            if (ativoViewModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.Ativo = ativoViewModel.Nome;

            return View(ativoViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, AtivoViewModel ativoViewModel)
        {
            if (id != ativoViewModel.Id) return HttpNotFound();

            if (!ModelState.IsValid) return View(ativoViewModel);

            Ativo ativo = _mapper.Map<Ativo>(ativoViewModel);
            await _ativoService.Update(ativo);

            if (!OperacaoValida()) return View(_mapper.Map<AtivoViewModel>(await _ativoService.GetById(id)));

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Details(Guid id)
        {
            AtivoViewModel ativoViewModel = _mapper.Map<AtivoViewModel>(await _ativoService.GetById(id));
            if (ativoViewModel == null)
            {
                return HttpNotFound();
            }

            return View(ativoViewModel);
        }

        public async Task<ActionResult> Delete(Guid id)
        {
            AtivoViewModel ativoViewModel = _mapper.Map<AtivoViewModel>(await _ativoService.GetById(id));
            if (ativoViewModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.Ativo = ativoViewModel.Nome;

            return View(ativoViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            AtivoViewModel ativoViewModel = _mapper.Map<AtivoViewModel>(await _ativoService.GetById(id));

            if (ativoViewModel == null) return HttpNotFound();

            await _ativoService.DeleteById(id);

            if (!OperacaoValida()) return View(ativoViewModel);

            return RedirectToAction("Index");
        }

    }
}