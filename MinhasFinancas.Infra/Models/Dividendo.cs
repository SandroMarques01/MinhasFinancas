using System;

namespace MinhasFinancas.Infra.Models
{
    public class Dividendo : Entity
    {
        public Guid PapelId { get; set; }
        public double ValorRecebido { get; set; }
        public double Quantidade { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public TipoDividendo TipoDividendo { get; set; }
        public string Historico { get; set; }

        /* Relações EF */
        public virtual Papel Papel { get; set; }
    }
}