using Fcg.Domain.Entities;
using Fcg.Infrastructure.Repositories;
using Fcg.Infrastructure.Tests.Fakers;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Infrastructure.Tests
{
    [Trait("Domain-infrastructure", "Game Repository")]
    public class GameRepositoryTests : BaseRepositoryTests
    {
        private readonly GameRepository _gameRepository;

        public GameRepositoryTests()
        {
            // Supondo que GameRepository exista e siga o mesmo padrão de UserRepository
            _gameRepository = new GameRepository(_context);
        }

        [Fact]
        public void CreateAsync_ShouldThrowArgumentException_WhenCreatingGameWithNegativePrice()
        {
            // Arrange
            Action act = () => new Game("Test Game", "Test Description", GenreEnum.Acao, -9.99m);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }





        //[Fact]
        //public async Task UpdateAsync_ShouldFailSilently_WhenGameDoesNotExist()
        //{
        //    // Arrange
        //    var nonExistentGame = EntityFakers.GameFaker.Generate();

        //    // Act
        //    // Supondo que exista um método UpdateAsync no repositório.
        //    Func<Task> act = async () => await _gameRepository.UpdateAsync(nonExistentGame);

        //    // Assert
        //    // A operação não deve lançar uma exceção, pois é uma operação idempotente.
        //    // O EF Core, por padrão, não lança exceção se a entidade a ser atualizada não for rastreada.
        //    await act.Should().NotThrowAsync();
        //}
    }
}