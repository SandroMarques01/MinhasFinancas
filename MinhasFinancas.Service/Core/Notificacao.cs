using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MinhasFinancas.Service.Core
{
    public class Notificacao
    {
        public Notificacao(string mensagem)
        {
            Mensagem = mensagem;
        }

        public string Mensagem { get; }
    }
}