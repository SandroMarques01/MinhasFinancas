using MinhasFinancas.Repository.Papel;
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

        public PapelService(IPapelRepository baseRepository,
                            ITransacaoService transacaoService,
                            IDividendoService dividendoService,
                            INotificador notificador) : base(notificador)
        {
            _baseRepository = baseRepository;
            _transacaoService = transacaoService;
            _dividendoService = dividendoService;
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
            var transacoes = await _transacaoService.Get(f=>f.PapelId == origem);
            transacoes.ToList().ForEach(t => {
                t.PapelId = destino;
                _transacaoService.Update(t);
            });
            #endregion

            #region troca de dividendos
            var dividendos = await _dividendoService.Get(f => f.PapelId == origem);
            dividendos.ToList().ForEach(t =>
            {
                t.PapelId = destino;
                _dividendoService.Update(t);
            });
            #endregion
        }

    }
}