using System.ComponentModel.DataAnnotations;
using System;
using MinhasFinancas.Infra;
using System.Collections.Generic;

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
        public Guid PapelId { get; set; }
        public double ValorUnt { get; set; }
        public int Quantidade { get; set; }
        public DateTime Data { get; set; }
        public TipoTransacao TipoTransacao { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }

        /* Relações EF */
        public virtual PapelViewModel Papel { get; set; }

        /// <summary>
        /// Propriedades fora da classe
        /// </summary>
        public virtual IEnumerable<PapelViewModel> Papels { get; set; }
    }
}