using MinhasFinancas.Infra.Models;
using System.Data.Entity.ModelConfiguration;

namespace MinhasFinancas.Infra.Mappings
{
    public class PapelConfig : EntityTypeConfiguration<Papel>
    {
        public PapelConfig()
        {
            HasKey(p => p.Id);

            Property(f => f.Codigo)
                .IsRequired()
                .HasMaxLength(10);

            Property(f => f.Nome)
                .IsRequired();

            Property(f => f.TipoPapel)
                .IsRequired();

            Property(f => f.Descricao)
                .HasMaxLength(5000);

            Property(f => f.Ativo)
                .IsRequired();

            HasMany(f => f.Segmento).WithMany(e => e.Papel);

            ToTable("TbPapel");
        }
    }
}