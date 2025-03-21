using MinhasFinancas.Infra;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Web.ViewModels
{
	public class InventarioViewModel
    {
        public InventarioViewModel()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public Guid AtivoId { get; set; }
        public double Valor { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public bool StatusAtivo { get; set; }

        public virtual AtivoViewModel Ativo { get; set; }

        /// <summary>
        /// Propriedades fora da classe
        /// </summary>
        public virtual IEnumerable<AtivoViewModel> Ativos { get; set; }

    }
}