using Fcg.Domain.Entities;
using Fcg.Domain.Queries;
using Fcg.Domain.Queries.Responses;
using Fcg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Infrastructure.Queries
{
    public class UserQuery : IUserQuery
    {
        private readonly FcgDbContext _context;

        public UserQuery(FcgDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            var users = await (from u in _context.Users
                               select new UserResponse
                               {
                                   Id = u.Id,
                                   Name = u.Name,
                                   Email = u.Email,
                                   Role = u.Role
                               }).ToListAsync();

            return users;
        }

        public async Task<UserResponse?> GetByIdUserAsync(Guid userId)
        {
            var user = await (from u in _context.Users
                              where u.Id == userId
                              select new UserResponse
                              {
                                  Id = u.Id,
                                  Name = u.Name,
                                  Email = u.Email,
                                  Role = u.Role
                              }).FirstOrDefaultAsync();

            return user;
        }

        public async Task<IEnumerable<GameResponse>> GetLibraryByUserAsync(Guid userId)
        {
            var games = await (from ug in _context.UserGamings
                               where ug.UserId == userId
                               join g in _context.Games on ug.GameId equals g.Id
                               select new GameResponse
                               {
                                   Id = g.Id,
                                   Title = g.Title,
                                   Description = g.Description,
                                   Genre = (GenreEnum)g.Genre,
                                   Price = g.Price,
                                   CreatedAt = g.CreatedAt
                               }).ToListAsync();

            return games;
        }
    }
}
