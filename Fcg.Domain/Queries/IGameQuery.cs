using Fcg.Domain.Queries.Responses;

namespace Fcg.Domain.Queries
{
    public interface IGameQuery
    {
        Task<IEnumerable<GameResponse>> GetAllGamesAsync();
        Task<GameResponse?> GetByIdGameAsync(Guid gameId);
    }
}
