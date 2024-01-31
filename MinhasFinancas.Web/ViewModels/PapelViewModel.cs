using MinhasFinancas.Infra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc.Routing.Constraints;

namespace MinhasFinancas.Web.ViewModels
{
    public class PapelViewModel
    {
        public PapelViewModel()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        [DisplayName("Código")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Tipo")]
        public TipoPapel TipoPapel { get; set; }

        [Range(0, Double.PositiveInfinity)]
        [DisplayName("Cotação atual")]
        public double CotacaoAtual { get; set; }

        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Ativo")]
        public bool Ativo { get; set; }

        public virtual IEnumerable<TransacaoViewModel> Transacao { get; set; }
        public virtual IEnumerable<DividendoViewModel> Dividendo { get; set; }
        public virtual IEnumerable<SegmentoViewModel> Segmento { get; set; }

        /// <summary>
        /// Propriedades fora da classe
        /// </summary>
        public int QuantidadeTotal { get; set; }
        public double TotalSaldo { get; set; }
        public double TotalSaldoAtual { get; set; }
        public double PrecoMedio { get; set; }
        public double DividendosTotal { get; set; }
        public double PercentDividendos { get; set; }
    }
}