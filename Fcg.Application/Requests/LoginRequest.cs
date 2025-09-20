using Fcg.Application.Responses;
using FluentValidation;
using MediatR;

namespace Fcg.Application.Requests
{
    public class LoginRequest : IRequest<LoginResponse>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).NotNull()
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("Formato de e-mail inválido.");

            RuleFor(x => x.Password).NotNull()
                .NotEmpty().WithMessage("A senha é obrigatória.");
        }
    }
}
