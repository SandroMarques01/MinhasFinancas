using MinhasFinancas.Infra.Models;
using System.Data.Entity.ModelConfiguration;

namespace MinhasFinancas.Infra.Mappings 
{
    public class SegmentoConfig : EntityTypeConfiguration<Segmento>
    {

        public SegmentoConfig()
        {
            HasKey(d => d.Id);

            Property(f => f.Nome)
                .IsRequired();

            Property(f => f.Descricao)
                .HasMaxLength(5000);

            Property(f => f.Ativo)
                .IsRequired();


            HasMany(f => f.Papel).WithMany(e => e.Segmento);

            ToTable("TbSegmento");
        }
    }
}