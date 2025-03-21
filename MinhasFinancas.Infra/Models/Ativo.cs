using System;
using System.Collections.Generic;

namespace MinhasFinancas.Infra.Models
{
	public class Ativo : Entity
    {
        public Guid LoginId { get; set; }
        public string Nome { get; set; }
        public TipoAtivo TipoAtivo { get; set; }
        public string Descricao { get; set; }
        public bool StatusAtivo { get; set; }

        public virtual ICollection<Inventario> Inventario { get; set; }

        public virtual Login Login { get; set; }

    }
}