using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MinhasFinancas.Web.ViewModels
{
    public class TrocaPapelViewModel
    {
        public Guid PapelIdOrigem { get; set; }
        public virtual IEnumerable<PapelViewModel> PapelsOrigem { get; set; }
        public Guid PapelIdDestino { get; set; }
        public virtual IEnumerable<PapelViewModel> PapelsDestino { get; set; }
    }
}