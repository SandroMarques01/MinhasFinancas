using MinhasFinancas.Repository.Dividendo;
using MinhasFinancas.Repository.Papel;
using MinhasFinancas.Repository.Transacao;
using MinhasFinancas.Service.Core;
using MinhasFinancas.Service.Dividendo;
using MinhasFinancas.Service.Transacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MinhasFinancas.Service.Papel
{
    public class PapelService : BaseService, IPapelService
    {
        private readonly IPapelRepository _baseRepository;
        private readonly ITransacaoService _transacaoService;
        private readonly IDividendoService _dividendoService;
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly IDividendoRepository _dividendoRepository;

        public PapelService(IPapelRepository baseRepository,
                            ITransacaoService transacaoService,
                            IDividendoService dividendoService,
                            ITransacaoRepository transacaoRepository,
                            IDividendoRepository dividendoRepository,
                            INotificador notificador) : base(notificador)
        {
            _baseRepository = baseRepository;
            _transacaoService = transacaoService;
            _dividendoService = dividendoService;
            _transacaoRepository = transacaoRepository;
            _dividendoRepository = dividendoRepository;
        }

        public async Task Add(Infra.Models.Papel obj)
        {
            if (!ExecutarValidacao(new PapelValidation(), obj)) return;

            await _baseRepository.Add(obj);
            await _baseRepository.SaveChanges();
        }

        public async Task DeleteById(Guid id)
        {
            await _baseRepository.DeleteById(id);
            await _baseRepository.SaveChanges();
        }

        public void Dispose()
        {
            //_baseRepository?.Dispose();
            //_baseRepository?.Dispose();
        }

        public async Task<IEnumerable<Infra.Models.Papel>> Get(Expression<Func<Infra.Models.Papel, bool>> filter = null, string includeProperties = null)
        {
            var retorno = await _baseRepository.Get(filter: filter, includeProperties: includeProperties);
            return retorno;
        }

        public async Task<IEnumerable<Infra.Models.Papel>> GetDividendosSemana(string includeProperties = null)
        {
            var retorno = await _baseRepository.Get(filter: null, includeProperties: includeProperties);
            return retorno;
        }

        public async Task<Infra.Models.Papel> GetById(Guid id)
        {
            var retorno = await _baseRepository.GetById(id);
            return retorno;
        }

        public async Task Update(Infra.Models.Papel obj)
        {
            if (!ExecutarValidacao(new PapelValidation(), obj)) return;

            await _baseRepository.Update(obj);
            await _baseRepository.SaveChanges();
        }

        public async Task TrocaPapel(Guid origem, Guid destino)
        {
            #region troca de Transações
            var transacoes = await _transacaoService.Get(f => f.PapelId == origem);
            if (transacoes.ToList().Any())
            {
                foreach (var transacao in transacoes)
                {
                    transacao.PapelId = destino;
                    await _transacaoRepository.Update(transacao);
                }
                await _baseRepository.SaveChanges();
            }

            #endregion

            #region troca de dividendos
            var dividendos = await _dividendoService.Get(f => f.PapelId == origem);
            if (dividendos.ToList().Any())
            {
                foreach (var dividendo in dividendos)
                {
                    dividendo.PapelId = destino;
                    await _dividendoRepository.Update(dividendo);
                }
                await _baseRepository.SaveChanges();
            }
            #endregion
            Infra.Models.Papel papel = await GetById(origem);
            papel.Ativo = false;
            await _baseRepository.Update(papel);
            await _baseRepository.SaveChanges();
        }

    }
}