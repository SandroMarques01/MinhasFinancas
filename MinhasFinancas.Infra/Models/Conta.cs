using System;
using System.Collections.Generic;

namespace MinhasFinancas.Infra.Models
{
	public class Conta : Entity
    {
        public Guid LoginId { get; set; }
        public string Nome { get; set; }
        public double ValorEstimado{ get; set; }
        public int DiaPagamento { get; set; }
        public string Descricao { get; set; }
        public bool StatusAtivo { get; set; }

        public virtual ICollection<Pagamento> Pagamento { get; set; }

        public virtual Login Login { get; set; }
    }
}