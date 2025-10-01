using MediatR;
using Fcg.Application.Responses;

namespace Fcg.Application.Requests
{
    public class CreateAdminUserRequest : IRequest<CreateAdminUserResponse>
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}