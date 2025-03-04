using MinhasFinancas.Infra.Models;
using System.Data.Entity.ModelConfiguration;

namespace MinhasFinancas.Infra.Mappings
{
    public class DividendosConfig : EntityTypeConfiguration<Dividendo>
    {
        public DividendosConfig()
        {
            HasKey(d => d.Id);

            Property(d => d.ValorRecebido)
                .IsRequired();

            Property(d => d.Quantidade)
                .IsRequired();

            Property(d => d.Data)
                .IsRequired();

            Property(f => f.Descricao)
                .HasMaxLength(5000);

            Property(f => f.Ativo)
                .IsRequired();

            Property(f => f.TipoDividendo)
                .IsRequired();

            Property(f => f.Historico)
                .HasColumnType("nvarchar(max)");

            HasRequired(d => d.Papel)
                .WithMany(p => p.Dividendo)
                .HasForeignKey(d => d.PapelId);

            ToTable("TbDividendo");
        }
    }
}