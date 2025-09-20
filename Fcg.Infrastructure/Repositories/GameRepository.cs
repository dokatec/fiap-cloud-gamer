using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using Fcg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly FcgDbContext _context;

        public GameRepository(FcgDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateGameAsync(Game game)
        {
            var entity = new Tables.Game
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                CreatedAt = game.CreatedAt,
                Genre = (int)game.Genre,
                Price = game.Price
            };

            _context.Games.Add(entity);

            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<Game?> GetGameByTitleAsync(string title)
        {
            var game = await (from g in _context.Games
                              where g.Title == title
                              select new Game(
                                  g.Id,
                                  g.Title,
                                  g.Description,
                                  (GenreEnum)g.Genre,
                                  g.Price,
                                  g.CreatedAt))
                              .FirstOrDefaultAsync();

            return game;
        }

        public async Task<IEnumerable<Game>?> GetGamesByIdsAsync(IEnumerable<Guid> guids)
        {
            var games = await (from g in _context.Games
                               where guids.Contains(g.Id)
                               select new Game(
                                   g.Id,
                                   g.Title,
                                   g.Description,
                                   (GenreEnum)g.Genre,
                                   g.Price,
                                   g.CreatedAt))
                              .ToListAsync();

            return games;
        }

        public async Task<Game?> GetGameByIdAsync(Guid gameId)
        {
            var game = await (from g in _context.Games
                              where g.Id == gameId
                              select new Game(
                                  g.Id,
                                  g.Title,
                                  g.Description,
                                  (GenreEnum)g.Genre,
                                  g.Price,
                                  g.CreatedAt))
                              .FirstOrDefaultAsync();

            return game;
        }
        public async Task<bool> DeleteGameAsync(Guid gameId)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            await _context.UserGamings
                .Where(ug => ug.GameId == gameId)
                .ExecuteDeleteAsync();

            var affected = await _context.Games
                .Where(g => g.Id == gameId)
                .ExecuteDeleteAsync();

            await tx.CommitAsync();
            return affected > 0;
        }
    }
}