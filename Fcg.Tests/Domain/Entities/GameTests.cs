using Xunit;
using Fcg.Domain.Entities;
using Bogus;
using System;

namespace Fcg.Tests.UnitTests
{

    [Trait("Domain-Entity", "Games")]
    public class GameTests
    {
        private readonly Faker<Game> _gameFaker;

        public GameTests()
        {
            _gameFaker = new Faker<Game>()
                .CustomInstantiator(f => new Game(
                    f.Commerce.ProductName(),
                    f.Lorem.Sentence(),
                    f.PickRandom<GenreEnum>(),
                    f.Finance.Amount(10, 1000)
                ));
        }

        [Fact]
        public void Game_Constructor_WithValidArguments_ShouldCreateGame()
        {
            // Arrange
            var game = _gameFaker.Generate();

            // Act & Assert
            Assert.NotEqual(Guid.Empty, game.Id);
            Assert.False(string.IsNullOrWhiteSpace(game.Title));
            Assert.False(string.IsNullOrWhiteSpace(game.Description));
            Assert.True(Enum.IsDefined(typeof(GenreEnum), game.Genre));
            Assert.True(game.Price >= 0);
            Assert.NotEqual(default(DateTime), game.CreatedAt);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Game_Constructor_WithInvalidTitle_ShouldThrowArgumentException(string invalidTitle)
        {
            // Arrange
            var faker = _gameFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Game(
                invalidTitle,
                faker.Description,
                faker.Genre,
                faker.Price
            ));
            Assert.True(exception.Message.Contains("Título não pode ser vazio ou nulo")==true);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Game_Constructor_WithInvalidDescription_ShouldThrowArgumentException(string invalidDescription)
        {
            // Arrange
            var faker = _gameFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Game(
                faker.Title,
                invalidDescription,
                faker.Genre,
                faker.Price
            ));
            Assert.Contains("Descrição não pode ser vazio ou nulo", exception.Message);
        }

        [Theory]
        [InlineData((GenreEnum)0)] 
        [InlineData((GenreEnum)999)] 
        public void Game_Constructor_WithInvalidGenre_ShouldThrowArgumentException(GenreEnum invalidGenre)
        {
            // Arrange
            var faker = _gameFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Game(
                faker.Title,
                faker.Description,
                invalidGenre,
                faker.Price
            ));
            Assert.True(exception.Message.Contains("Gênero inválido.")==true);
        }

        [Fact]
        public void Game_Constructor_WithNegativePrice_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var faker = _gameFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Game(
                faker.Title,
                faker.Description,
                faker.Genre,
                -10m
            ));
            Assert.True(exception.Message.Contains("Preço não pode ser menor que 0. (Parameter 'price')") ==true);
        }

        [Fact]
        public void UpdateDetails_WithValidArguments_ShouldUpdateProperties()
        {
            // Arrange
            var game = _gameFaker.Generate();
            var newTitle = "New Game Title";
            var newDescription = "New Game Description";
            var newGenre = GenreEnum.Aventura;

            // Act
            game.UpdateDetails(newTitle, newDescription, newGenre);

            // Assert
            Assert.Equal(newTitle, game.Title);
            Assert.Equal(newDescription, game.Description);
            Assert.Equal(newGenre, game.Genre);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void UpdateDetails_WithInvalidNewTitle_ShouldThrowArgumentException(string invalidTitle)
        {
            // Arrange
            var game = _gameFaker.Generate();
            var newDescription = "New Game Description";
            var newGenre = GenreEnum.Aventura;

            invalidTitle = string.Empty; 
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => game.UpdateDetails(invalidTitle, newDescription, newGenre));
            Assert.Contains("Título não pode ser vazio ou nulo", exception.Message);
        }

        [Fact]
        public void ChangePrice_WithValidPrice_ShouldUpdatePrice()
        {
            // Arrange
            var game = _gameFaker.Generate();
            var newPrice = 99.99m;

            // Act
            game.ChangePrice(newPrice);

            // Assert
            Assert.Equal(newPrice, game.Price);
        }

        [Fact]
        public void ChangePrice_WithNegativePrice_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var game = _gameFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => game.ChangePrice(-5.0m));
            Assert.Contains("Preço não pode ser negativo. (Parameter 'newPrice')", exception.Message);
        }
    }
}