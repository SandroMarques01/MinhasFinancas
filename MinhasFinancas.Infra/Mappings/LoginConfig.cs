using MinhasFinancas.Infra.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace MinhasFinancas.Infra.Mappings
{
	public class LoginConfig : EntityTypeConfiguration<Login>
    {
        public LoginConfig()
        {
            HasKey(p => p.Id);

            Property(f => f.Nome)
                .IsRequired();

            Property(f => f.Usuario)
                .IsRequired();

            Property(f => f.Senha)
                .IsRequired();

            ToTable("TbLogin");
        }
    }
}