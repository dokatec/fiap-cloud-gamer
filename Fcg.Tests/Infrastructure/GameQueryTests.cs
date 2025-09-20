using Fcg.Infrastructure.Data;
using Fcg.Infrastructure.Queries;
using Fcg.Infrastructure.Tables;
using Fcg.Infrastructure.Tests.Fakers;
using FluentAssertions;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Fcg.Domain.Entities;
using Game = Fcg.Infrastructure.Tables.Game;

namespace Fcg.Infrastructure.Tests.Queries
{
    [Trait("Domain-infrastructure", "Game Queries")]
    public class GameQueryTests : BaseRepositoryTests
    {
        private readonly GameQuery _gameQuery;

        public GameQueryTests()
        {
            _gameQuery = new GameQuery(_context);
        }

        [Fact]
        public async Task GetAllGamesAsync_ShouldReturnAllGames()
        {
            // Arrange
            var gamesData = EntityFakers.GameFaker.Generate(2);
            _context.Games.AddRange(gamesData.Select(g => new Game { Id = g.Id, Title = g.Title, Description = g.Description, Genre = (int)g.Genre, Price = g.Price, CreatedAt = g.CreatedAt }));
            await _context.SaveChangesAsync();

            // Act
            var result = await _gameQuery.GetAllGamesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(g => g.Title == gamesData[0].Title);
            result.Should().Contain(g => g.Title == gamesData[1].Title);
        }

        [Fact]
        public async Task GetAllGamesAsync_ShouldReturnEmptyList_WhenNoGamesExist()
        {
            // Arrange
            // No games in the database

            // Act
            var result = await _gameQuery.GetAllGamesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByIdGameAsync_ShouldReturnGame_WhenGameExists()
        {
            // Arrange
            var gameData = EntityFakers.GameFaker.Generate();
            _context.Games.Add(new Game { Id = gameData.Id, Title = gameData.Title, Description = gameData.Description, Genre = (int)gameData.Genre, Price = gameData.Price, CreatedAt = gameData.CreatedAt });
            await _context.SaveChangesAsync();

            // Act
            var result = await _gameQuery.GetByIdGameAsync(gameData.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(gameData.Id);
            result.Title.Should().Be(gameData.Title);
        }

        [Fact]
        public async Task GetByIdGameAsync_ShouldReturnNull_WhenGameDoesNotExist()
        {
            // Act
            var result = await _gameQuery.GetByIdGameAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }
    }
}