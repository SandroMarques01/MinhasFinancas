﻿using MinhasFinancas.Infra;
using MinhasFinancas.Web.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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

        public Guid LoginId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        [DisplayName("#")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Tipo")]
        public TipoPapel TipoPapel { get; set; }


        [Range(0, Double.PositiveInfinity)]
        [DisplayName("Cot. atual")]
        public double CotacaoAtual { get; set; }

        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [DisplayName("Ativo")]
        public bool Ativo { get; set; }

        public virtual IEnumerable<TransacaoViewModel> Transacao { get; set; }
        public virtual IEnumerable<DividendoViewModel> Dividendo { get; set; }
        public virtual IEnumerable<SegmentoViewModel> Segmento { get; set; }
        public virtual LoginViewModel Login { get; set; }

        /// <summary>
        /// Propriedades fora da classe
        /// </summary>
        [DisplayName("Qtd")]
        public double QuantidadeTotal { get; set; }
        [DisplayName("Total")]
        [Moeda]
        public double TotalSaldo { get; set; }
        [DisplayName("Total")] 
        public double TotalSaldoAtual { get; set; }
        [DisplayName("$ Médio")]
        [Moeda]
        public double PrecoMedio { get; set; }
        [DisplayName("Div.")]
        public double DividendosTotal { get; set; }
        [DisplayName("% DY.")]
        public double PercentDividendos { get; set; }
        [DisplayName("Val.")]
        public double Valorizacao { get; set; }
        [DisplayName("Ganho Unt")]
        public double GanhoUnt { get; set; }
        [DisplayName("Ganho T.")]
        public double GanhoTotal { get; set; }


        [DisplayName("% DoC.")]
        public double PercentDivOnCost { get; set; }
    }
}