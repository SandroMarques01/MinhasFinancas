using FluentValidation;

namespace MinhasFinancas.Service.Segmento
{
    public class SegmentoValidation : AbstractValidator<Infra.Models.Segmento>
    {
        public SegmentoValidation()
        {

            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio")
                .Length(5, 100).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(d => d.Ativo)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio");
        }
    }
}