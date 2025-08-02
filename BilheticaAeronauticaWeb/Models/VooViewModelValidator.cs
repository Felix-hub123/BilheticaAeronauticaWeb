using System.ComponentModel.DataAnnotations;
using FluentValidation;
using System;


namespace BilheticaAeronauticaWeb.Models
{
  

    public class VooViewModelValidator : AbstractValidator<VooViewModel>
    {
        public VooViewModelValidator()
        {
            RuleFor(v => v.DataHoraPartida)
                .NotEmpty().WithMessage("A data/hora de partida é obrigatória.")
                .Must(BeFutureDate).WithMessage("A data/hora de partida deve ser futura.");

            RuleFor(v => v.DataHoraChegada)
                .NotEmpty().WithMessage("A data/hora de chegada é obrigatória.")
                .GreaterThan(v => v.DataHoraPartida).WithMessage("A data/hora de chegada deve ser posterior à de partida.");

            RuleFor(v => v.OrigemId)
                .NotEmpty().WithMessage("Selecione a origem.");

            RuleFor(v => v.DestinoId)
                .NotEmpty().WithMessage("Selecione o destino.")
                .NotEqual(v => v.OrigemId).WithMessage("A origem e o destino não podem ser iguais.");

            RuleFor(v => v.AviaoId)
                .NotEmpty().WithMessage("Selecione o avião.");

            RuleFor(v => v.PrecoBase)
                .GreaterThan(0).WithMessage("O preço base deve ser maior que zero.");
        }

        public static ValidationResult ValidarDatas(VooViewModel voo, System.ComponentModel.DataAnnotations.ValidationContext context)
        {
            if (voo.DataHoraChegada <= voo.DataHoraPartida)
            {
                return new ValidationResult(
                    "A data/hora de chegada deve ser posterior à de partida.",
                    new[] { nameof(voo.DataHoraChegada) });
            }

            if (voo.OrigemId == voo.DestinoId)
            {
                return new ValidationResult(
                    "A origem e o destino não podem ser iguais.",
                    new[] { nameof(voo.DestinoId) });
            }

            return ValidationResult.Success;
        }

        private bool BeFutureDate(DateTime date)
        {
            return date > DateTime.UtcNow;
        }
    }
}