using MinhasFinancas.Service.Core;
using MinhasFinancas.Repository.Login;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq;

namespace MinhasFinancas.Service.Login
{
    public class LoginService : BaseService, ILoginService
    {
        private readonly ILoginRepository _baseRepository;

        public LoginService(ILoginRepository baseRepository, INotificador notificador) : base(notificador)
        {
            _baseRepository = baseRepository;
        }

        public async Task Add(Infra.Models.Login obj)
        {
            if (!ExecutarValidacao(new LoginValidation(), obj)) return;

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

        public async Task<IEnumerable<Infra.Models.Login>> Get(Expression<Func<Infra.Models.Login, bool>> filter = null, string includeProperties = null)
        {
            var retorno = await _baseRepository.Get(filter: filter, includeProperties: includeProperties);
            return retorno;
        }

        public async Task<Infra.Models.Login> GetById(Guid id)
        {
            var retorno = await _baseRepository.GetById(id);
            return retorno;
        }

        public async Task<Infra.Models.Login> GetByUsuarioSenha(string usuario, string senha)
        {
            var retorno = await _baseRepository.Get(x => x.Usuario == usuario && x.Senha == senha);
            return retorno.FirstOrDefault();
        }


        public async Task Update(Infra.Models.Login obj)
        {
            if (!ExecutarValidacao(new LoginValidation(), obj)) return;

            await _baseRepository.Update(obj);
            await _baseRepository.SaveChanges();
        }

    }
}