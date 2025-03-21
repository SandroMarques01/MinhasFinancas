using FluentValidation;

namespace MinhasFinancas.Service.Pagamento
{
    public class PagamentoValidation : AbstractValidator<Infra.Models.Pagamento>
    {
        public PagamentoValidation()
        {
            RuleFor(p => p.ValorPago)
                .GreaterThanOrEqualTo(0).WithMessage("O campo {PropertyName} precisa ser maior que {ComparisonValue}");

            RuleFor(p => p.Data)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio");


            //When(p => p.TipoPapel == TipoPapel.BDR, () => {});
        }
    }
}