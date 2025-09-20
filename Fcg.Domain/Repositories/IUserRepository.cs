using Fcg.Domain.Entities;

namespace Fcg.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<Guid> CreateUserAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(Guid id);
        Task UpdateUserProfileAsync(User user);
        Task UpdateUserRoleAsync(Guid userId, string newRole);
        Task UpdateUserLibraryAsync(User user);
        Task<bool> DeleteUserAsync(Guid userId);
    }
}