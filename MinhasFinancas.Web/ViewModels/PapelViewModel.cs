using MinhasFinancas.Infra;
using System;
using System.Collections.Generic;
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
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public TipoPapel TipoPapel { get; set; }
        public double CotacaoAtual { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }

        public virtual IEnumerable<TransacaoViewModel> Transacaos { get; set; }
        public virtual IEnumerable<DividendoViewModel> Dividendos { get; set; }
        public virtual IEnumerable<SegmentoViewModel> Segmentos { get; set; }
    }
}