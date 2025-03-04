using System.Collections.Generic;

namespace MinhasFinancas.Infra.Models
{
    public class Login : Entity
    {
        public string Nome { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public virtual ICollection<Papel> Papel { get; set; }
    }
}