using MinhasFinancas.Infra.Models;
using System.Data.Entity.ModelConfiguration;

namespace MinhasFinancas.Infra.Mappings
{
	public class AtivoConfig : EntityTypeConfiguration<Ativo>
    {
		public AtivoConfig()
        {
            HasKey(p => p.Id);

            Property(f => f.Nome)
                .IsRequired()
                .HasMaxLength(200);

            Property(f => f.TipoAtivo)
                .IsRequired();

            Property(f => f.Descricao)
                .HasMaxLength(5000);

            Property(f => f.StatusAtivo)
                .IsRequired();

            HasMany(f => f.Inventario).WithRequired(e => e.Ativo);

            HasRequired(t => t.Login)
                .WithMany(p => p.Ativo)
                .HasForeignKey(t => t.LoginId);

            ToTable("TbAtivo");
        }
    }
}