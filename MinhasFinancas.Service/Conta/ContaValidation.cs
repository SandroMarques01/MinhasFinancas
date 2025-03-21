using FluentValidation;

namespace MinhasFinancas.Service.Conta
{
    public class ContaValidation : AbstractValidator<Infra.Models.Conta>
    {
        public ContaValidation()
        {
            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio");

            RuleFor(p => p.ValorEstimado)
                .GreaterThanOrEqualTo(0).WithMessage("O campo {PropertyName} precisa ser maior que {ComparisonValue}");

            RuleFor(p => p.DiaPagamento)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio");


            //When(p => p.TipoPapel == TipoPapel.BDR, () => {});
        }
    }
}