using AutoMapper;
using Microsoft.Ajax.Utilities;
using MinhasFinancas.Service.Core;
using MinhasFinancas.Service.Login;
using MinhasFinancas.Web.ViewModels;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MinhasFinancas.Web.Area.Login.Controllers
{
    public class LoginController : Controller
    {
        ILoginService _loginService;
        IMapper _mapper;

        public LoginController(ILoginService loginService,
                                    IMapper mapper,
                                    INotificador notificador)
        {
            _loginService = loginService;
            _mapper = mapper;
        }
        // GET: Login
        public async Task<ActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginViewModel log)
        {
            LoginViewModel user = _mapper.Map<LoginViewModel>(await _loginService.GetByUsuarioSenha(log.Usuario, log.Senha));

            if (user != null)
            {
                ConfigurationManager.AppSettings.Set("User", user.Nome);
                ConfigurationManager.AppSettings.Set("UserId", user.Id.ToString());
                return Redirect(@"/Home/Index");
            }

            return View();
        }
    }
}