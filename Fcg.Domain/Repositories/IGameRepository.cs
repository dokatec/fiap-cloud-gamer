using Fcg.Domain.Entities;

namespace Fcg.Domain.Repositories
{
    public interface IGameRepository
    {
        Task<Guid> CreateGameAsync(Game game);
        Task<Game?> GetGameByTitleAsync(string title);
        Task<IEnumerable<Game>?> GetGamesByIdsAsync(IEnumerable<Guid> guids);
        Task<Game?> GetGameByIdAsync(Guid gameId);
        Task<bool> DeleteGameAsync(Guid gameId);
    }
}