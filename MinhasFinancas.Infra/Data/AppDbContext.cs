using MinhasFinancas.Infra.Mappings;
using MinhasFinancas.Infra.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MinhasFinancas.Infra.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("MinhasFinancasDB")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            //modelBuilder.Entity<Papel>().ToTable("TbPapel");
            //modelBuilder.Entity<Transacao>().ToTable("TbTransacao");
            //modelBuilder.Entity<Dividendo>().ToTable("TbDividendo");
            //modelBuilder.Entity<Segmento>().ToTable("TbSegmento");

            modelBuilder.Properties<string>().Configure(p => p.HasColumnType("varchar").HasMaxLength(100));

            modelBuilder.Configurations.Add(new LoginConfig());
            modelBuilder.Configurations.Add(new PapelConfig());
            modelBuilder.Configurations.Add(new TransacaoConfig());
            modelBuilder.Configurations.Add(new DividendosConfig());
            modelBuilder.Configurations.Add(new SegmentoConfig());

            modelBuilder.Configurations.Add(new AtivoConfig());
            modelBuilder.Configurations.Add(new InventarioConfig());
            modelBuilder.Configurations.Add(new ContaConfig());
            modelBuilder.Configurations.Add(new PagamentoConfig());

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Login> Logins { get; set; }
        public DbSet<Papel> Papels { get; set; }
        public DbSet<Transacao> Transacaos { get; set; }
        public DbSet<Dividendo> Dividendos { get; set; }
        public DbSet<Segmento> Segmentos { get; set; }

        public DbSet<Ativo> Ativos { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }
        public DbSet<Conta> Contas { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }
    }
}