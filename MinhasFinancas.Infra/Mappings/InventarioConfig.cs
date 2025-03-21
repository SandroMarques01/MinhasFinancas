using MinhasFinancas.Infra.Models;
using System.Data.Entity.ModelConfiguration;

namespace MinhasFinancas.Infra.Mappings
{
	public class InventarioConfig : EntityTypeConfiguration<Inventario>
    {
		public InventarioConfig()
		{
			HasKey(t => t.Id);

            Property(t => t.Valor)
                .IsRequired();

            Property(t => t.Data)
                .IsRequired();

            Property(t => t.Descricao)
                .HasMaxLength(5000);

            Property(f => f.StatusAtivo)
                .IsRequired();

            HasRequired(t => t.Ativo)
                .WithMany(p => p.Inventario)
                .HasForeignKey(t => t.AtivoId);

            ToTable("TbInventario");
        }

	}
}