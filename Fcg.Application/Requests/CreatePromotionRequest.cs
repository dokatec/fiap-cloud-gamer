using Fcg.Application.Responses;
using Fcg.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Fcg.Application.Requests
{
    public class CreatePromotionRequest : IRequest<CreatePromotionResponse>
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public GenreEnum Genre { get; set; }

    }

    public class CreatePromotionRequestValidator : AbstractValidator<CreatePromotionRequest>
    {
        public CreatePromotionRequestValidator()
        {
            RuleFor(p => p.Title)
                .NotEmpty()
                .WithMessage("O título da promoção é obrigatório.");

            RuleFor(p => p.Description)
                .NotEmpty()
                .WithMessage("A descrição da promoção é obrigatória.");

            RuleFor(p => p.DiscountPercent)
                .InclusiveBetween(0.01m, 100m)
                .WithMessage("O desconto deve ser entre 0.01 e 100 (percentual).");

            RuleFor(p => p.StartDate)
                .NotEmpty()
                .WithMessage("A data de início da promoção é obrigatória.");

            RuleFor(p => p.EndDate)
                .NotEmpty()
                .WithMessage("A data de término da promoção é obrigatória.")
                .GreaterThan(p => p.StartDate)
                .WithMessage("A data de término deve ser posterior à data de início.");

            RuleFor(p => p.Genre)
                .IsInEnum()
                .WithMessage("O gênero informado não é válido.");
        }
    }
}
