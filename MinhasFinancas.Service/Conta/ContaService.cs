using MinhasFinancas.Repository.Conta;
using MinhasFinancas.Service.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MinhasFinancas.Service.Conta
{
    public class ContaService : BaseService, IContaService
    {
        private readonly IContaRepository _baseRepository;

        public ContaService(IContaRepository baseRepository, INotificador notificador) : base(notificador)
        {
            _baseRepository = baseRepository;
        }

        public async Task Add(Infra.Models.Conta obj)
        {
            if (!ExecutarValidacao(new ContaValidation(), obj)) return;

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

        public async Task<IEnumerable<Infra.Models.Conta>> Get(Expression<Func<Infra.Models.Conta, bool>> filter = null, string includeProperties = null)
        {
            var retorno = await _baseRepository.Get(filter: filter, includeProperties: includeProperties);
            return retorno;
        }

        public async Task<Infra.Models.Conta> GetById(Guid id)
        {
            var retorno = await _baseRepository.GetById(id);
            return retorno;
        }

        public async Task Update(Infra.Models.Conta obj)
        {
            if (!ExecutarValidacao(new ContaValidation(), obj)) return;

            await _baseRepository.Update(obj);
            await _baseRepository.SaveChanges();
        }

    }
}