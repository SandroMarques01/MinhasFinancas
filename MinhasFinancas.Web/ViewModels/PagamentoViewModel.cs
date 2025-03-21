using MinhasFinancas.Infra;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Web.ViewModels
{
	public class PagamentoViewModel
    {
        public PagamentoViewModel()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public Guid ContaId { get; set; }
        public double ValorPago { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public bool StatusAtivo { get; set; }

        public virtual ICollection<ContaViewModel> Conta { get; set; }

    }
}