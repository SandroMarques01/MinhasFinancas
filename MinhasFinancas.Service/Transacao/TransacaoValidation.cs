using FluentValidation;

namespace MinhasFinancas.Service.Transacao
{
    public class TransacaoValidation : AbstractValidator<Infra.Models.Transacao>
    {
        public TransacaoValidation()
        {
            RuleFor(t => t.Data)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio"); //Validar depois

            RuleFor(t => t.TipoTransacao)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio");
        }
    }
}