using Xunit;
using Fcg.Domain.Entities;
using Bogus;
using System;
using System.Linq;

namespace Fcg.Tests.UnitTests
{
    [Trait("Domain-Entity", "User Gaming")]
    public class UserGamingTests
    {
        private readonly Faker<User> _userFaker;
        private readonly Faker<Game> _gameFaker;

        public UserGamingTests()
        {
            _userFaker = new Faker<User>()
                .CustomInstantiator(f => new User(
                    f.Person.FullName,
                    f.Internet.Email(),
                    f.PickRandom("User", "Admin")
                ));

            _gameFaker = new Faker<Game>()
                .CustomInstantiator(f => new Game(
                    f.Commerce.ProductName(),
                    f.Lorem.Sentence(),
                    f.PickRandom<GenreEnum>(),
                    f.Finance.Amount(10, 1000)
                ));
        }

        [Fact]
        public void UserGaming_Constructor_WithValidArguments_ShouldCreateUserGaming()
        {
            // Arrange
            var user = _userFaker.Generate();
            var game = _gameFaker.Generate();

            // Act
            var userGaming = new UserGaming(user, game);

            // Assert
            Assert.NotEqual(Guid.Empty, userGaming.Id);
            Assert.Equal(user, userGaming.User);
            Assert.Equal(game, userGaming.Game);
            Assert.NotEqual(default(DateTime), userGaming.PurchasedDate);
            Assert.True(userGaming.PurchasedDate.Kind == DateTimeKind.Utc);
        }

        [Fact]
        public void UserGaming_Constructor_WithNullUser_ShouldThrowArgumentNullException()
        {
            // Arrange
            var game = _gameFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new UserGaming(null, game));
            Assert.Contains("User cannot be null.", exception.Message);
        }

        [Fact]
        public void UserGaming_Constructor_WithNullGame_ShouldThrowArgumentNullException()
        {
            // Arrange
            var user = _userFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new UserGaming(user, null));
            Assert.Contains("Game cannot be null.", exception.Message);
        }

        [Fact]
        public void UserGaming_RehydrationConstructor_WithValidArguments_ShouldCreateUserGaming()
        {
            // Arrange
            var id = Guid.NewGuid();
            var user = _userFaker.Generate();
            var game = _gameFaker.Generate();
            var purchasedDate = DateTime.Now.AddDays(-10);

            // Act
            var userGaming = new UserGaming(id, user, game, purchasedDate);

            // Assert
            Assert.Equal(id, userGaming.Id);
            Assert.Equal(user, userGaming.User);
            Assert.Equal(game, userGaming.Game);
            Assert.Equal(purchasedDate.ToUniversalTime(), userGaming.PurchasedDate);
            Assert.True(userGaming.PurchasedDate.Kind == DateTimeKind.Utc);
        }

        [Fact]
        public void UserGaming_RehydrationConstructor_WithEmptyId_ShouldThrowArgumentException()
        {
            // Arrange
            var user = _userFaker.Generate();
            var game = _gameFaker.Generate();
            var purchasedDate = DateTime.Now;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new UserGaming(Guid.Empty, user, game, purchasedDate));
            Assert.Contains("Id não pode ser vazio. (Parameter 'id')", exception.Message);
        }

        [Fact]
        public void UserGaming_RehydrationConstructor_WithDefaultPurchasedDate_ShouldThrowArgumentException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var user = _userFaker.Generate();
            var game = _gameFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new UserGaming(id, user, game, default(DateTime)));
            Assert.Contains("PurchasedDate deve ser preenchida.", exception.Message);
        }
    }
}