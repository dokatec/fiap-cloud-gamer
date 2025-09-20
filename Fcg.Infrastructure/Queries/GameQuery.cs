using Fcg.Domain.Entities;
using Fcg.Domain.Queries;
using Fcg.Domain.Queries.Responses;
using Fcg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Infrastructure.Queries
{
    public class GameQuery : IGameQuery
    {
        private readonly FcgDbContext _context;

        public GameQuery(FcgDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GameResponse>> GetAllGamesAsync()
        {
            var games = await(from g in _context.Games
                              select new GameResponse
                              {
                                  Id = g.Id,
                                  Title = g.Title,
                                  Description = g.Description,
                                  Genre = (GenreEnum)g.Genre,
                                  CreatedAt = g.CreatedAt,
                                  Price = g.Price
                              }).ToListAsync();

            return games;
        }

        public async Task<GameResponse?> GetByIdGameAsync(Guid gameId)
        {
            var game = await(from g in _context.Games
                              where g.Id == gameId
                              select new GameResponse
                              {
                                  Id = g.Id,
                                  Title = g.Title,
                                  Description = g.Description,
                                  Genre = (GenreEnum)g.Genre,
                                  CreatedAt = g.CreatedAt,
                                  Price = g.Price
                              }).FirstOrDefaultAsync();

            return game;
        }
    }
}
