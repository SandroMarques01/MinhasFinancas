using System.Collections.Generic;

namespace MinhasFinancas.Infra.Models
{
    public class Papel : Entity
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public TipoPapel TipoPapel { get; set;}
        public double CotacaoAtual { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }

        public virtual ICollection<Transacao> Transacaos { get; set; }
        public virtual ICollection<Dividendo> Dividendos { get; set; }
        public virtual ICollection<Segmento> Segmentos { get; set; }


        /// <summary>
        /// Propriedades fora da classe
        /// </summary>
        public int QuantidadeTotal { get; set; }
    }
}