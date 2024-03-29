﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Nome")]
        public string Nome { get; set; }

        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Ativo")]
        public bool Ativo { get; set; }


        public virtual IEnumerable<PapelViewModel> Papel { get; set; }


        /// <summary>
        /// Propriedades fora da classe
        /// </summary>
        /// 
    }
}