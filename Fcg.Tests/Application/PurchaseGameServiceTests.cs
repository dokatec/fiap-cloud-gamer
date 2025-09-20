using Fcg.Application.Services;
using Fcg.Domain.Repositories;
using Fcg.Infrastructure.Repositories; // Mantém para a instanciação
using Fcg.Infrastructure.Tests.Fakers;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Infrastructure.Tests.Application
{
    [Trait("Application-service", "PurchaseGameService")]
    public class PurchaseGameServiceTests : BaseRepositoryTests, IDisposable
    {
        private readonly IUserRepository _userRepository;
        private readonly IGameRepository _gameRepository;
        private readonly PurchaseGameService _purchaseGameService;

        public PurchaseGameServiceTests()
        {
            _userRepository = new UserRepository(_context);
            _gameRepository = new GameRepository(_context);
            _purchaseGameService = new PurchaseGameService(_userRepository, _gameRepository);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldAddGameToUserLibrary_WhenPurchaseIsSuccessful()
        {
            // Arrange
            var user = EntityFakers.UserFaker.Generate();
            var game = EntityFakers.GameFaker.Generate();

            await _userRepository.CreateUserAsync(user);
            await _gameRepository.CreateGameAsync(game);

            // Act
            await _purchaseGameService.ExecuteAsync(user.Id, game.Id);

            // Assert
            var updatedUser = await _userRepository.GetUserByIdAsync(user.Id);
            updatedUser.Should().NotBeNull();
            updatedUser.Library.Should().ContainSingle(ug => ug.Game.Id == game.Id);
        }

        

        [Fact]
        public async Task ExecuteAsync_ShouldThrowInvalidOperationException_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentUserId = Guid.NewGuid();
            var game = EntityFakers.GameFaker.Generate();
            await _gameRepository.CreateGameAsync(game);

            // Act
            Func<Task> act = async () => await _purchaseGameService.ExecuteAsync(nonExistentUserId, game.Id);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("User not found.");
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowInvalidOperationException_WhenGameDoesNotExist()
        {
            // Arrange
            var user = EntityFakers.UserFaker.Generate();
            await _userRepository.CreateUserAsync(user);
            var nonExistentGameId = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await _purchaseGameService.ExecuteAsync(user.Id, nonExistentGameId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Game not found.");
        }
    }
}