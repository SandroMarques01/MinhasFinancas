using System;

namespace MinhasFinancas.Infra.Models
{
	public class Inventario : Entity
    {
        public Guid AtivoId { get; set; }
        public double Valor { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public bool StatusAtivo { get; set; }

        public virtual Ativo Ativo { get; set; }
    }
}