using DocumentFormat.OpenXml.Office2010.Excel;
using MinhasFinancas.Infra.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MinhasFinancas.Web.ViewModels
{
	public class LoginViewModel
    {
        public LoginViewModel()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public string Nome { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public virtual ICollection<Papel> Papel { get; set; }

    }
}