using Fcg.Application.Requests;
using Fcg.Application.Responses;
using Fcg.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Application.Handlers
{
    public class UpdateRoleHandler : IRequestHandler<UpdateRoleRequest, UpdateRoleResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UpdateRoleHandler> _logger;

        public UpdateRoleHandler(ILogger<UpdateRoleHandler> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<UpdateRoleResponse> Handle(UpdateRoleRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.UserId);

            if (user == null)
            {
                _logger.LogWarning("Usuário com id {UserId} não encontrado!", request.UserId);

                return new UpdateRoleResponse
                {
                    Success = false,
                    Message = "Usuário não encontrado."
                };
            }

            user.SetRole(request.NewRole);

            await _userRepository.UpdateUserRoleAsync(user.Id, user.Role);

            return new UpdateRoleResponse
            {
                Success = true,
                Message = "Cargo atualizado com sucesso!",
                UserId = user.Id,
                Role = user.Role
            };
        }
    }
}
