using Fcg.Application.Responses;
using FluentValidation;
using MediatR;
using System.Text.RegularExpressions;

namespace Fcg.Application.Requests
{
    public class CreateUserRequest : IRequest<CreateUserResponse>
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(p => p.Name).NotNull().MinimumLength(3)
                .WithMessage("O nome deve ter pelo menos 3 caracteres.");
            RuleFor(p => p.Email).NotNull().EmailAddress().WithMessage("Formato de e-mail inválido.");
            RuleFor(p => p.Password).NotEmpty().NotNull()
            .Must(BeAStrongPassword)
            .WithMessage("A senha não é forte o suficiente. Ela deve ter no mínimo 8 caracteres, incluindo letras maiúsculas, minúsculas, números e um símbolo.");
        }

        private bool BeAStrongPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return true;
            }

            var strongPasswordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");

            return strongPasswordRegex.IsMatch(password);
        }
    }
}
