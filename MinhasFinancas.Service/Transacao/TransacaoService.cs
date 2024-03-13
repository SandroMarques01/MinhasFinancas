using MinhasFinancas.Repository.Transacao;
using MinhasFinancas.Service.Core;
using MinhasFinancas.Service.Transacao;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;
using MinhasFinancas.Service.Papel;
using System.Linq;
using MinhasFinancas.Repository.Papel;

namespace MinhasFinancas.Service.Transacao
{
    public class TransacaoService : BaseService, ITransacaoService
    {
        private readonly ITransacaoRepository _baseRepository;
        private readonly IPapelRepository _papelRepository;

        public TransacaoService(ITransacaoRepository baseRepository, IPapelRepository papelRepository, INotificador notificador) : base(notificador)
        {
            _baseRepository = baseRepository;
            _papelRepository = papelRepository;
        }

        public async Task Add(Infra.Models.Transacao obj)
        {
            if (!ExecutarValidacao(new TransacaoValidation(), obj)) return;

            var transPapel = await _baseRepository.Get(x => x.PapelId == obj.PapelId);
            if (transPapel.Sum(x=> x.Quantidade * (x.TipoTransacao == Infra.TipoTransacao.Venda ? -1 : 1)) == 0)
            {
                var papel = await _papelRepository.GetById(obj.PapelId);
                papel.Ativo = false;
                await _papelRepository.Update(papel);
            }

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

        public async Task<IEnumerable<Infra.Models.Transacao>> Get(Expression<Func<Infra.Models.Transacao, bool>> filter = null, string includeProperties = null)
        {
            var retorno = await _baseRepository.Get(filter: filter, includeProperties: includeProperties);
            return retorno;
        }

        public async Task<Infra.Models.Transacao> GetById(Guid id)
        {
            var retorno = await _baseRepository.GetById(id);
            return retorno;
        }

        public async Task Update(Infra.Models.Transacao obj)
        {
            if (!ExecutarValidacao(new TransacaoValidation(), obj)) return;

            var transPapel = await _baseRepository.Get(x => x.PapelId == obj.PapelId);
            if (transPapel.Sum(x => x.Quantidade * (x.TipoTransacao == Infra.TipoTransacao.Venda ? -1 : 1)) == 0)
            {
                var papel = await _papelRepository.GetById(obj.PapelId);
                papel.Ativo = false;
                await _papelRepository.Update(papel);
            }

            await _baseRepository.Update(obj);
            await _baseRepository.SaveChanges();
        }
    }
}