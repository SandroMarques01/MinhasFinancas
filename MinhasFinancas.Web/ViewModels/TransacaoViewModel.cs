﻿using System.ComponentModel.DataAnnotations;
using System;
using MinhasFinancas.Infra;
using System.Collections.Generic;
using System.ComponentModel;

namespace MinhasFinancas.Web.ViewModels
{
    public class TransacaoViewModel
    {
        public TransacaoViewModel()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Papel")]
        public Guid PapelId { get; set; }

        [Range(0, Double.PositiveInfinity)]
        [DisplayName("Valor Unitário")]
        public double ValorUnt { get; set; }

        [Range(0, Double.PositiveInfinity)]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Quantidade")]
        public double Quantidade { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Data")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Tipo")]
        public TipoTransacao TipoTransacao { get; set; }

        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [DisplayName("Ativo")]
        public bool Ativo { get; set; }

        public string Historico { get; set; }

        /* Relações EF */
        public virtual PapelViewModel Papel { get; set; }

        /// <summary>
        /// Propriedades fora da classe
        /// </summary>
        public virtual IEnumerable<PapelViewModel> Papels { get; set; }

        [DisplayName("Evolução")]
        public double Evolucao { get; set; }
        [DisplayName("Evolução Atual")]
        public double EvolucaoAtual { get; set; }
    }
}