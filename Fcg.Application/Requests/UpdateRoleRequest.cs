using Fcg.Application.Responses;
using FluentValidation;
using MediatR;

namespace Fcg.Application.Requests
{
    public class UpdateRoleRequest : IRequest<UpdateRoleResponse>
    {
        public Guid UserId { get; set; }
        public string NewRole { get; set; } = null!;
    }

    public class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
    {
        public UpdateRoleRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("O ID do usuário é obrigatório.");
            RuleFor(x => x.NewRole)
                .NotEmpty().WithMessage("O novo papel é obrigatório.")
                .Must(role => role == "Admin" || role == "User")
                .WithMessage("O papel deve ser 'Admin' ou 'User'.");
        }
    }
}
