using Fcg.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace Fcg.Application.Services
{
    public class UserManagementService
    {
        private readonly IUserRepository _userRepository;

        public UserManagementService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ChangeUserRoleAsync(Guid performerId, Guid targetUserId, string newRole)
        {
            var performer = await _userRepository.GetUserByIdAsync(performerId);
            if (performer?.Role != "Admin")
            {
                throw new UnauthorizedAccessException("User is not authorized to perform this action.");
            }

            var targetUser = await _userRepository.GetUserByIdAsync(targetUserId);
            if (targetUser == null)
            {
                throw new InvalidOperationException("Target user not found.");
            }

            // 1. Diga à entidade de domínio para executar a lógica de negócio
            targetUser.SetRole(newRole);

            // 2. Diga ao repositório para persistir a entidade atualizada
            await _userRepository.UpdateUserRoleAsync(targetUser.Id, newRole);
        }
    }
}