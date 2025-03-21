using AutoMapper;
using MinhasFinancas.Infra.Models;
using MinhasFinancas.Service.Ativo;
using MinhasFinancas.Service.Core;
using MinhasFinancas.Service.Inventario;
using MinhasFinancas.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MinhasFinancas.Web.Controllers
{
    public class InventarioController : BaseController
    {
        IInventarioService _inventarioService;
        IAtivoService _ativoService;
        IMapper _mapper;

        public InventarioController(IInventarioService inventarioService,
                                    IAtivoService ativoService,
                                    IMapper mapper,
                                    INotificador notificador) : base(notificador)
        {
            _inventarioService = inventarioService;
            _ativoService = ativoService;
            _mapper = mapper;
        }


        public async Task<ActionResult> Index()
        {
            if (string.IsNullOrEmpty(userId))
                return Redirect(@"/Login/Index");

            List<InventarioViewModel> lst = _mapper.Map<List<InventarioViewModel>>(await _inventarioService.Get(x => x.Ativo.LoginId.ToString() == userId, includeProperties: "Ativo")).ToList();

            return View(lst);
        }


        [HttpGet]
        public async Task<ActionResult> Details(Guid id)
        {
            InventarioViewModel inventarioViewModel = _mapper.Map<InventarioViewModel>(await _inventarioService.GetById(id));
            if (inventarioViewModel == null)
            {
                return HttpNotFound();
            }

            inventarioViewModel.Ativo = _mapper.Map<List<AtivoViewModel>>(await _ativoService.Get(f => f.Id == inventarioViewModel.AtivoId && f.LoginId.ToString() == userId)).FirstOrDefault();
            ViewBag.Ativo = inventarioViewModel.Ativo.Nome + " - " + inventarioViewModel.Data.ToString("dd/MM/yyyy");
            return View(inventarioViewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var inventarioViewModel = PopularAtivos(new InventarioViewModel());

            return View(inventarioViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InventarioViewModel inventarioViewModel)
        {
            if (!ModelState.IsValid) return View(inventarioViewModel);

            Inventario obj = _mapper.Map<Inventario>(inventarioViewModel);
            await _inventarioService.Add(obj);

            if (!OperacaoValida()) return View(inventarioViewModel);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(Guid id)
        {
            InventarioViewModel inventarioViewModel = _mapper.Map<InventarioViewModel>(await _inventarioService.GetById(id));
            if (inventarioViewModel == null)
            {
                return HttpNotFound();
            }

            inventarioViewModel.Ativo = _mapper.Map<List<AtivoViewModel>>(await _ativoService.Get(f => f.Id == inventarioViewModel.AtivoId && f.LoginId.ToString() == userId)).FirstOrDefault();
            inventarioViewModel = await PopularAtivos(inventarioViewModel);
            ViewBag.Ativo = inventarioViewModel.Ativo.Nome + " | " + inventarioViewModel.Data.ToString("dd/MM/yyyy");

            return View(inventarioViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, InventarioViewModel inventarioViewModel)
        {
            if (id != inventarioViewModel.Id) return HttpNotFound();

            if (!ModelState.IsValid) return View(inventarioViewModel);

            Inventario Inventario = _mapper.Map<Inventario>(inventarioViewModel);
            await _inventarioService.Update(Inventario);

            if (!OperacaoValida()) return View(_mapper.Map<InventarioViewModel>(await _inventarioService.GetById(id)));

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(Guid id)
        {
            InventarioViewModel inventarioViewModel = _mapper.Map<InventarioViewModel>(await _inventarioService.GetById(id));
            if (inventarioViewModel == null)
            {
                return HttpNotFound();
            }

            inventarioViewModel.Ativo = _mapper.Map<List<AtivoViewModel>>(await _ativoService.Get(f => f.Id == inventarioViewModel.AtivoId && f.LoginId.ToString() == userId)).FirstOrDefault();
            ViewBag.Ativo = inventarioViewModel.Ativo.Nome + " | " + inventarioViewModel.Data.ToString("dd/MM/yyyy");

            return View(inventarioViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            InventarioViewModel inventarioViewModel = _mapper.Map<InventarioViewModel>(await _inventarioService.GetById(id));

            if (inventarioViewModel == null) return HttpNotFound();

            await _inventarioService.DeleteById(id);

            if (!OperacaoValida()) return View(inventarioViewModel);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> CreateMult(string dtMes = null)
        {
            if (string.IsNullOrEmpty(userId))
                return Redirect(@"/Login/Index");

            List<AtivoViewModel> ativos = _mapper.Map<List<AtivoViewModel>>(await _ativoService.Get(x => x.LoginId.ToString() == userId));

            DateTime data1 = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-01");
            DateTime dataUltimo = data1.AddMonths(1).AddDays(-1);

            //List<InventarioViewModel> lst = _mapper.Map<List<InventarioViewModel>>(await _inventarioService.Get(x => x.Ativo.LoginId.ToString() == userId && x.Data == data1, includeProperties: "Ativo")).ToList();

            foreach (var ativo in ativos)
            {
                var inventario = _mapper.Map<List<InventarioViewModel>>(await _inventarioService.Get(x => x.AtivoId == ativo.Id && x.Data == data1));
                ativo.Inventario = inventario.FirstOrDefault();
            }

            return View(ativos);
        }

        [HttpPost]
        public async Task<ActionResult> CreateMult(List<InventarioViewModel> listInventarios)
        {


            return Redirect(@"/Index");
        }


        private async Task<InventarioViewModel> PopularAtivos(InventarioViewModel inventario)
        {
            inventario.Ativos = _mapper.Map<List<AtivoViewModel>>(await _ativoService.Get(x => x.LoginId.ToString() == userId));
            return inventario;
        }


    }
}