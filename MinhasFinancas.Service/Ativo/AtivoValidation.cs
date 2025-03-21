using FluentValidation;

namespace MinhasFinancas.Service.Ativo
{
    public class AtivoValidation : AbstractValidator<Infra.Models.Ativo>
    {
        public AtivoValidation()
        {
            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio");

            RuleFor(p => p.TipoAtivo)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio");

            //When(p => p.TipoPapel == TipoPapel.BDR, () => {});
        }
    }
}