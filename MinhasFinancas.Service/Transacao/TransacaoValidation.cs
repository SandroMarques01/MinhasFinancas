using FluentValidation;

namespace MinhasFinancas.Service.Transacao
{
    public class TransacaoValidation : AbstractValidator<Infra.Models.Transacao>
    {
        public TransacaoValidation()
        {
            RuleFor(t => t.ValorUnt)
                .GreaterThanOrEqualTo(0).WithMessage("O campo {PropertyName} precisa ser maior que {ComparisonValue}");

            RuleFor(t => t.Quantidade)
                .GreaterThanOrEqualTo(0).WithMessage("O campo {PropertyName} precisa ser maior que {ComparisonValue}");

            RuleFor(t => t.Data)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio"); //Validar depois

            RuleFor(t => t.TipoTransacao)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio");

            RuleFor(t => t.Ativo)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio");
        }
    }
}