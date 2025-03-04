using FluentValidation;

namespace MinhasFinancas.Service.Login
{
    public class LoginValidation : AbstractValidator<Infra.Models.Login>
    {
        public LoginValidation()
        {
            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio");

            RuleFor(p => p.Usuario)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio");


            RuleFor(p => p.Senha)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio");


            //When(p => p.TipoPapel == TipoPapel.BDR, () => {});
        }
    }
}