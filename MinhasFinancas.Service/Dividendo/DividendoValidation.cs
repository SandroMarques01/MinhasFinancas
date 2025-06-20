﻿using FluentValidation;

namespace MinhasFinancas.Service.Dividendo
{
    public class DividendoValidation : AbstractValidator<Infra.Models.Dividendo>
    {
        public DividendoValidation()
        {
            RuleFor(d => d.ValorRecebido)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio")
                .GreaterThan(0).WithMessage("O campo {PropertyName} precisa ser maior que {ComparisonValue}");

            RuleFor(d => d.Data)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio"); //Validar depois

            RuleFor(t => t.TipoDividendo)
                .NotEmpty().WithMessage("O campo {PropertyName} não pode estar vazio");
        }
    }
}