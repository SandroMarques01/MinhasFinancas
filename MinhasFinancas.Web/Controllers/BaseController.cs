using MinhasFinancas.Service.Core;
using MinhasFinancas.Web.ViewModels;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace MinhasFinancas.Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly INotificador _notificador;

        public BaseController(INotificador notificador)
        {
            _notificador = notificador;
        }

        public string userName
        {
            get { return ConfigurationManager.AppSettings.Get("User"); }
        }

        public string userId
        {
            get { return ConfigurationManager.AppSettings.Get("UserId"); }
        }

        protected bool OperacaoValida()
        {
            if (!_notificador.TemNotificacao()) return true;

            var notificacoes = _notificador.ObterNotificacoes();
            notificacoes.ForEach(c => ViewData.ModelState.AddModelError(string.Empty, c.Mensagem));
            return false;
        }

        //protected void IsActiveUser()
        //{
        //    if (string.IsNullOrEmpty(userId))
        //        return Redirect(@"/Login/Index");
        //}

    }
}