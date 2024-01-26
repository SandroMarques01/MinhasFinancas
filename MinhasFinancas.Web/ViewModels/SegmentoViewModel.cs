using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Web.ViewModels
{
    public class SegmentoViewModel
    {
        public SegmentoViewModel()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }


        public virtual IEnumerable<PapelViewModel> Papels { get; set; }


        /// <summary>
        /// Propriedades fora da classe
        /// </summary>
        /// 
    }
}