﻿using System;
using System.Collections.Generic;

namespace MinhasFinancas.Infra.Models
{
    public class Papel : Entity
    {
        public Guid LoginId { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public TipoPapel TipoPapel { get; set;}
        public double CotacaoAtual { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }

        public virtual ICollection<Transacao> Transacao { get; set; }
        public virtual ICollection<Dividendo> Dividendo { get; set; }
        public virtual ICollection<Segmento> Segmento { get; set; }

        public virtual Login Login { get; set; }

    }
}