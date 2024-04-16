using MinhasFinancas.Infra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Web.ViewModels
{
    public class DividendoViewModel
    {
        public DividendoViewModel()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Papel")]
        public Guid PapelId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Range(0, Double.PositiveInfinity)]
        [DisplayName("Valor Recebido")]
        public double ValorRecebido { get; set; }

        [Range(0, Double.PositiveInfinity)]
        [DisplayName("Qtd")]
        public int Quantidade { get; set; }
        
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Data")]
        public DateTime Data { get; set; }

        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [DisplayName("Ativo")]
        public bool Ativo { get; set; }
        public TipoDividendo TipoDividendo { get; set; }

        /* Relações EF */
        public virtual PapelViewModel Papel { get; set; }

        /// <summary>
        /// Propriedades fora da classe
        /// </summary>
        public virtual IEnumerable<PapelViewModel> Papels { get; set; }
        [DisplayName("DY%")]
        public double YieldOnCost { get; set; }
        [DisplayName("$ Médio")]
        public double PrecoMedio { get; set; }

        [DisplayName("Evolução")]
        public double Evolucao { get; set; }


    }
}