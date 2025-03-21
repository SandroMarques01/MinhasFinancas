using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using MinhasFinancas.Infra.Models;
using MinhasFinancas.Service.Configuracao;
using MinhasFinancas.Service.Core;
using MinhasFinancas.Service.Login;
using MinhasFinancas.Web.ViewModels;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MinhasFinancas.Web.Controllers
{
    public class ConfiguracaoController : BaseController
    {
        IConfiguracaoService _configuracaoService;
        ILoginService _loginService;
        IMapper _mapper;

        public ConfiguracaoController(IConfiguracaoService configuracaoService,
                                        ILoginService loginService,
                                        IMapper mapper,
                                        INotificador notificador) : base(notificador)
        {
            _configuracaoService = configuracaoService;
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

            await _configuracaoService.ImportarExcelB3(fileB3, userId);

            return RedirectToAction("Index", "Papel");
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
    }
}