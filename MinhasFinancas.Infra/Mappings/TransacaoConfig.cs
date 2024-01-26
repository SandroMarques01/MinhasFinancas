using MinhasFinancas.Infra.Models;
using System.Data.Entity.ModelConfiguration;

namespace MinhasFinancas.Infra.Mappings
{
    public class TransacaoConfig : EntityTypeConfiguration<Transacao>
    {
        public TransacaoConfig()
        {
            HasKey(t => t.Id);

            Property(f => f.ValorUnt)
                .IsRequired();

            Property(f => f.TipoTransacao)
                .IsRequired();

            Property(f => f.Quantidade)
                .IsRequired();

            Property(f => f.Descricao)
                .HasMaxLength(5000);

            Property(f => f.Ativo)
                .IsRequired();

            HasRequired(t => t.Papel)
                .WithMany(p => p.Transacao)
                .HasForeignKey(t => t.PapelId);

            ToTable("TbTransacao");
        }
    }
}