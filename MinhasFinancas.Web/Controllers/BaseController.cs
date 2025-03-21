using MinhasFinancas.Infra;
using MinhasFinancas.Service.Core;
using MinhasFinancas.Web.ViewModels;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
            get { return Session["User"] as string; }
        }

        public string userId
        {
            get { return Session["UserId"] as string; }
        }

        protected bool OperacaoValida()
        {
            if (!_notificador.TemNotificacao()) return true;

            var notificacoes = _notificador.ObterNotificacoes();
            notificacoes.ForEach(c => ViewData.ModelState.AddModelError(string.Empty, c.Mensagem));
            return false;
        }

        protected double CalculoPrecoMedio(List<TransacaoViewModel> transacoes)
        {
            double precoMedio = 0;
            double valorYotal = 0;
            double qtdYotal = 0;
            foreach (var transacao in transacoes.OrderBy(o => o.Data))
            {
                if (transacao.TipoTransacao == TipoTransacao.Compra)
                {
                    valorYotal += transacao.Quantidade * transacao.ValorUnt;
                    qtdYotal += transacao.Quantidade;
                    precoMedio = valorYotal / qtdYotal;
                }
                else
                {
                    qtdYotal -= transacao.Quantidade;
                    valorYotal = precoMedio * qtdYotal;
                }
            }

            return precoMedio;
        }

        //protected void IsActiveUser()
        //{
        //    if (string.IsNullOrEmpty(userId))
        //        return Redirect(@"/Login/Index");
        //}

    }
}