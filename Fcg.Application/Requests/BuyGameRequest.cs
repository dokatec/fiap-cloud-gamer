using Fcg.Application.Responses;
using FluentValidation;
using MediatR;

namespace Fcg.Application.Requests
{
    public class BuyGameRequest : IRequest<BuyGameResponse>
    {
        public Guid UserId { get; set; }
        public IEnumerable<Guid> GamesIds { get; set; } = null!;
    }

    public class BuyGameRequestValidator : AbstractValidator<BuyGameRequest>
    {
        public BuyGameRequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Usuário é obrigatório!");
            RuleFor(x => x.GamesIds).NotNull().WithMessage("A lista de jogos não pode ser vazia!");
        }
    }
}
