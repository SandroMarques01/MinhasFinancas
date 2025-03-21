using MinhasFinancas.Infra.Models;
using System.Data.Entity.ModelConfiguration;

namespace MinhasFinancas.Infra.Mappings
{
	public class PagamentoConfig : EntityTypeConfiguration<Pagamento>
    {
		public PagamentoConfig()
        {
            HasKey(x => x.Id);

            Property(x => x.ValorPago)
                .IsRequired();

            Property(x => x.Data)
                .IsRequired();

            Property(x => x.Descricao)
                .HasMaxLength(5000);

            Property(f => f.StatusAtivo)
                .IsRequired();

            HasRequired(x => x.Conta)
                .WithMany(x => x.Pagamento)
                .HasForeignKey(x => x.ContaId);

            ToTable("TbPagamento");
        }

    }
}