using System;
using System.Collections.Generic;

namespace MinhasFinancas.Web.ViewModels
{
    public class TrocaPapelViewModel
    {
        public Guid PapelIdOrigem { get; set; }
        public virtual IEnumerable<PapelViewModel> PapelsOrigem { get; set; }
        public Guid PapelIdDestino { get; set; }
        public virtual IEnumerable<PapelViewModel> PapelsDestino { get; set; }

        public int QuantidadeDesdobro { get; set; }
        public DateTime DataDesdobro { get; set; }
    }
}