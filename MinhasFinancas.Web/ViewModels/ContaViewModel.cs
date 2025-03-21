using MinhasFinancas.Infra;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Web.ViewModels
{
	public class ContaViewModel
    {
        public ContaViewModel()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public Guid LoginId { get; set; }
        public string Nome { get; set; }
        public double ValorEstimado { get; set; }
        public int DiaPagamento { get; set; }
        public string Descricao { get; set; }
        public bool StatusAtivo { get; set; }

        public virtual ICollection<PagamentoViewModel> Pagamento { get; set; }


        public virtual LoginViewModel Login { get; set; }

    }
}