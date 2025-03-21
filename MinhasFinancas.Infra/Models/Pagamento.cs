using System;

namespace MinhasFinancas.Infra.Models
{
	public class Pagamento : Entity
    {
        public Guid ContaId { get; set; }
        public double ValorPago { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public bool StatusAtivo { get; set; }

        public virtual Conta Conta { get; set; }

	}
}