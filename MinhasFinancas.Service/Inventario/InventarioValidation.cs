using FluentValidation;

namespace MinhasFinancas.Service.Inventario
{
    public class InventarioValidation : AbstractValidator<Infra.Models.Inventario>
    {
        public InventarioValidation()
        {
            RuleFor(p => p.Valor)
                .GreaterThanOrEqualTo(0).WithMessage("O campo {PropertyName} precisa ser maior que {ComparisonValue}");

            RuleFor(p => p.Data)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio");


            //When(p => p.TipoPapel == TipoPapel.BDR, () => {});
        }
    }
}