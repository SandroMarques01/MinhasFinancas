using MinhasFinancas.Infra.Models;
using System.Data.Entity.ModelConfiguration;

namespace MinhasFinancas.Infra.Mappings
{
	public class ContaConfig : EntityTypeConfiguration<Conta>
    {
        public ContaConfig()
        {
            HasKey(x => x.Id);

            Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(200);

            Property(x => x.ValorEstimado)
                .IsRequired();

            Property(x => x.DiaPagamento)
                .IsRequired();

            Property(t => t.Descricao)
                .HasMaxLength(5000);

            Property(f => f.StatusAtivo)
                .IsRequired();

            HasRequired(t => t.Login)
                .WithMany(p => p.Conta)
                .HasForeignKey(t => t.LoginId);

            ToTable("TbConta");
        }

    }
}