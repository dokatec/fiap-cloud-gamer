using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using Fcg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FcgDbContext _context;

        public UserRepository(FcgDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateUserAsync(User user)
        {
            var entity = new Tables.User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Role = user.Role
            };

            _context.Users.Add(entity);

            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var userEntity = await _context.Users
                .Include(u => u.Library!) 
                    .ThenInclude(ug => ug.Game) 
                .AsNoTracking() 
                .FirstOrDefaultAsync(u => u.Email == email);

            if (userEntity == null)
            {
                return null;
            }

            var domainUser = new User(
                userEntity.Id,
                userEntity.Name,
                userEntity.Email,
                userEntity.PasswordHash,
                userEntity.Library?.Select(ug => new UserGaming(
                    ug.Id, 
                    new User(userEntity.Id, userEntity.Name, userEntity.Email, userEntity.PasswordHash, new List<UserGaming>(), userEntity.Role), // Passa uma instância de User simplificada para UserGaming, ou null se não for essencial para o construtor
                    new Game(ug.Game.Id, ug.Game.Title, ug.Game.Description, (GenreEnum)ug.Game.Genre, ug.Game.Price, ug.Game.CreatedAt),
                    ug.PurchasedDate
                )) ?? new List<UserGaming>(),
                userEntity.Role
            );

            return domainUser;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            var userEntity = await _context.Users
                .Include(u => u.Library!) 
                    .ThenInclude(ug => ug.Game) 
                .AsNoTracking() 
                .FirstOrDefaultAsync(u => u.Id == id);

            if (userEntity == null)
            {
                return null;
            }

            var domainUser = new User(
                userEntity.Id,
                userEntity.Name,
                userEntity.Email,
                userEntity.PasswordHash,
                userEntity.Library?.Select(ug => new UserGaming(
                    ug.Id,
                    new User(userEntity.Id, userEntity.Name, userEntity.Email, userEntity.PasswordHash, new List<UserGaming>(), userEntity.Role), // Passa uma instância de User simplificada para UserGaming, ou null se não for essencial para o construtor
                    new Game(ug.Game.Id, ug.Game.Title, ug.Game.Description, (GenreEnum)ug.Game.Genre, ug.Game.Price, ug.Game.CreatedAt),
                    ug.PurchasedDate
                )) ?? new List<UserGaming>(),
                userEntity.Role
            );

            return domainUser;
        }

        public async Task UpdateUserLibraryAsync(User user)
        {
            if (user.GamesAdded != null && user.GamesAdded.Any())
            {
                var userGamingEntitiesToAdd = user.GamesAdded.Select(ug => new Tables.UserGaming
                {
                    Id = ug.Id, 
                    UserId = user.Id,
                    GameId = ug.Game.Id,
                    PurchasedDate = ug.PurchasedDate
                }).ToList(); 

                _context.UserGamings.AddRange(userGamingEntitiesToAdd);
            }

            if (user.GamesRemoved != null && user.GamesRemoved.Any())
            {
                var userGamingIdsToRemove = user.GamesRemoved.Select(ug => ug.Id).ToList();

                var existingUserGamingsToRemove = await _context.UserGamings
                    .Where(ug => userGamingIdsToRemove.Contains(ug.Id))
                    .ToListAsync();

                _context.UserGamings.RemoveRange(existingUserGamingsToRemove);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserRoleAsync(Guid userId, string newRole)
        {
            var userEntity = await _context.Users.FindAsync(userId);

            if (userEntity == null)
            {
                throw new InvalidOperationException($"Usuário com Id {userId} não encontrado para atualizar o papel.");
            }

            userEntity.Role = newRole;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserProfileAsync(User user)
        {
            var userEntity = await _context.Users.FindAsync(user.Id);

            if (userEntity == null)
            {
                throw new InvalidOperationException($"Usuário com Id {user.Id} não encontrado para atualizar o perfil.");
            }

            userEntity.Name = user.Name;
            userEntity.Email = user.Email;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var userEntity = await _context.Users
                .Include(u => u.Library)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (userEntity == null)
                return false;

            if (userEntity.Library != null && userEntity.Library.Count != 0)
            {
                _context.UserGamings.RemoveRange(userEntity.Library);
            }

            _context.Users.Remove(userEntity);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}