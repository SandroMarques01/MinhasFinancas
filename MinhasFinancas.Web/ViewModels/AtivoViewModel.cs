using MinhasFinancas.Infra;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Web.ViewModels
{
	public class AtivoViewModel
    {
        public AtivoViewModel()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public Guid LoginId { get; set; }
        public string Nome { get; set; }
        public TipoAtivo TipoAtivo { get; set; }
        public string Descricao { get; set; }
        public bool StatusAtivo { get; set; }

        public virtual ICollection<InventarioViewModel> Inventarios { get; set; }

        public virtual LoginViewModel Login { get; set; }

        /// <summary>
        /// Propriedades fora da classe
        /// </summary>
        public virtual InventarioViewModel Inventario { get; set; }


    }
}