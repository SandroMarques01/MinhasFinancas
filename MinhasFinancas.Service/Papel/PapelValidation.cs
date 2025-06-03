using FluentValidation;

namespace MinhasFinancas.Service.Papel
{
    public class PapelValidation : AbstractValidator<Infra.Models.Papel>
    {
        public PapelValidation()
        {
            RuleFor(p => p.Codigo)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio")
                .Length(5, 10).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio")
                .Length(2, 100).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(p => p.TipoPapel)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio");

            //When(p => p.TipoPapel == TipoPapel.BDR, () => {});
        }
    }
}