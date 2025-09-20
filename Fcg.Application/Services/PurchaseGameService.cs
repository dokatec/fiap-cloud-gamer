using Fcg.Domain.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Fcg.Application.Services
{
    public class PurchaseGameService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGameRepository _gameRepository;

        public PurchaseGameService(IUserRepository userRepository, IGameRepository gameRepository)
        {
            _userRepository = userRepository;
            _gameRepository = gameRepository;
        }

        public async Task ExecuteAsync(Guid userId, Guid gameId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var game = await _gameRepository.GetGameByIdAsync(gameId);
            if (game == null)
            {
                throw new InvalidOperationException("Game not found.");
            }

            user.AddGameToLibrary(game);

            await _userRepository.UpdateUserLibraryAsync(user);
        }
    }
}