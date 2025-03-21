using MinhasFinancas.Repository.Dividendo;
using MinhasFinancas.Repository.Login;
using MinhasFinancas.Repository.Papel;
using MinhasFinancas.Repository.Segmento;
using MinhasFinancas.Repository.Transacao;
using MinhasFinancas.Repository.Ativo;
using MinhasFinancas.Repository.Conta;
using MinhasFinancas.Repository.Inventario;
using MinhasFinancas.Repository.Pagamento;
using SimpleInjector;

namespace MinhasFinancas.Repository.Core
{
    public class DependencyInjectionConfigService
    {
        public static void InitializeContainer(Container container)
        {
            // Lifestyle.Singleton
            // Uma única instância por aplicação

            // Lifestyle.Transient
            // Cria uma nova instância para cada injeção

            //Lifestyle.Scoped
            // Uma única instância por request

            container.Register<IDividendoRepository, DividendoRepository>(Lifestyle.Scoped);
            container.Register<IPapelRepository, PapelRepository>(Lifestyle.Scoped);
            container.Register<ILoginRepository, LoginRepository>(Lifestyle.Scoped);
            container.Register<ISegmentoRepository, SegmentoRepository>(Lifestyle.Scoped);
            container.Register<ITransacaoRepository, TransacaoRepository>(Lifestyle.Scoped);

            container.Register<IAtivoRepository, AtivoRepository>(Lifestyle.Scoped);
            container.Register<IInventarioRepository, InventarioRepository>(Lifestyle.Scoped);
            container.Register<IContaRepository, ContaRepository>(Lifestyle.Scoped);
            container.Register<IPagamentoRepository, PagamentoRepository>(Lifestyle.Scoped);
        }
    }
}