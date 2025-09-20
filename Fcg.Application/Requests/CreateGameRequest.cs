using Fcg.Application.Responses;
using Fcg.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Fcg.Application.Requests
{
    public class CreateGameRequest : IRequest<CreateGameResponse>
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public GenreEnum Genre { get; set; }
        public decimal Price { get; set; }
    }

    public class CreateGameRequestValidator : AbstractValidator<CreateGameRequest>
    {
        public CreateGameRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MinimumLength(3).WithMessage("O nome deve ter pelo menos 3 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("A descrição é obrigatória.")
                .MinimumLength(50).WithMessage("A descrição deve ter pelo menos 50 caracteres.");

            RuleFor(x => x.Genre)
                .NotEmpty().WithMessage("A categoria é obrigatória.");

            RuleFor(x => x.Price)
              .GreaterThan(0).WithMessage("O preço deve ser maior que zero.");
        }       
    }
}
