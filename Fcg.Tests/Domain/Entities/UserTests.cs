using Bogus;
using Fcg.Domain.Entities;
using Fcg.Infrastructure.Tests.Fakers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fcg.Tests.UnitTests
{
    [Trait("Domain-Entity", "User")]
    public class UserTests
    {
        private readonly Faker<User> _userFaker;
        private readonly Faker<Game> _gameFaker;

        public UserTests()
        {
            _gameFaker = new Faker<Game>()
                .CustomInstantiator(f => new Game(
                    f.Commerce.ProductName(),
                    f.Lorem.Sentence(),
                    GenreEnum.Acao,
                    f.Finance.Amount(10, 1000)
                ));

            _userFaker = new Faker<User>()
                .CustomInstantiator(f => new User(
                    f.Person.FullName,
                    f.Internet.Email(),
                    f.PickRandom("User", "Admin")
                ));
        }

        [Fact]
        public void User_Constructor_WithValidArguments_ShouldCreateUser()
        {
            // Arrange
            var user = _userFaker.Generate();

            // Act & Assert
            Assert.NotEqual(Guid.Empty, user.Id);
            Assert.False(string.IsNullOrWhiteSpace(user.Name));
            Assert.False(string.IsNullOrWhiteSpace(user.Email));
            Assert.False(string.IsNullOrWhiteSpace(user.Role));
            Assert.NotNull(user.Library);
            Assert.Empty(user.Library);
            Assert.Empty(user.GamesAdded);
            Assert.Empty(user.GamesRemoved);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void User_Constructor_WithInvalidName_ShouldThrowArgumentException(string invalidName)
        {
            // Arrange
            var faker = _userFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new User(
                invalidName,
                faker.Email,
                faker.Role
            ));
            Assert.Contains("Nome não pode ser vazio ou nulo. (Parameter 'name')", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("invalid-email")]
        [InlineData("invalid@")]
        [InlineData("@invalid.com")]
        public void User_Constructor_WithInvalidEmail_ShouldThrowArgumentException(string invalidEmail)
        {
            // Arrange
            var faker = _userFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new User(
                faker.Name,
                invalidEmail,
                faker.Role
            ));
            Assert.True(exception.Message.Contains("Email não pode ser vazio ou nulo. (Parameter 'email')") ||
                        exception.Message.Contains("Formato de email é inválido (Parameter 'email')"),
                        $"Mensagem de exceção esperada: 'Email não pode ser vazio ou nulo.' ou 'Formato de email é inválido'. Mensagem real: '{exception.Message}'");
        }

        [Fact]
        public void AddGameToLibrary_NewGame_ShouldAddGameAndTrackAsAdded()
        {
            // Arrange
            var user = _userFaker.Generate();
            var game = _gameFaker.Generate();

            // Act
            user.AddGameToLibrary(game);

            // Assert
            Assert.Contains(user.Library, ug => ug.Game.Id == game.Id);
            Assert.Single(user.Library);
            Assert.Single(user.GamesAdded);
            Assert.Equal(game.Id, user.GamesAdded.First().Game.Id);
            Assert.Empty(user.GamesRemoved);
        }

        [Fact]
        public void AddGameToLibrary_ExistingGame_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var user = _userFaker.Generate();
            var game = _gameFaker.Generate();
            user.AddGameToLibrary(game); 

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => user.AddGameToLibrary(game));
            Assert.Contains($"O game com ID '{game.Id}' já está na biblioteca do usuário.", exception.Message);
            Assert.Single(user.Library); 
        }

        [Fact]
        public void AddGameToLibrary_NullGame_ShouldThrowArgumentNullException()
        {
            // Arrange
            var user = _userFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => user.AddGameToLibrary(null));
            Assert.Contains("Game não pode ser nulo. (Parameter 'game')", exception.Message);
        }

        [Fact]
        public void RemoveGameFromLibrary_ExistingGame_ShouldRemoveGameAndTrackAsRemoved()
        {
            // Arrange
            var game = _gameFaker.Generate();
            var initialUserGaming = new UserGaming(
                Guid.NewGuid(), 
                new User(Guid.NewGuid(), "Dummy User", "dummy@example.com", "hash", new List<UserGaming>(), "User"),
                game,
                DateTime.UtcNow
            );

            var user = new User(
                Guid.NewGuid(), // ID de usuário
                "Test User",
                "test@example.com",
                "hashedpassword",
                new List<UserGaming> { initialUserGaming }, // Biblioteca inicial populada
                "User"
            );

            // Verificaes iniciais para garantir o Arrange correto
            Assert.Contains(user.Library, ug => ug.Game.Id == game.Id);
            Assert.Empty(user.GamesAdded); // Deve estar vazio ao carregar do DB
            Assert.Empty(user.GamesRemoved); // Deve estar vazio ao carregar do DB

            // Act
            user.RemoveGameFromLibrary(game.Id);

            // Assert
            Assert.DoesNotContain(user.Library, ug => ug.Game.Id == game.Id);
            Assert.Empty(user.Library);
            Assert.Single(user.GamesRemoved);
            Assert.Equal(game.Id, user.GamesRemoved.First().Game.Id);
            Assert.Equal(initialUserGaming.Id, user.GamesRemoved.First().Id); // Verifica que é a mesma instância de UserGaming removida
            Assert.Empty(user.GamesAdded); // Certifique-se de que nada foi "adicionado" durante a remoção
        }


        [Fact]
        public void RemoveGameFromLibrary_NonExistingGame_ShouldNotThrowException()
        {
            // Arrange
            var user = EntityFakers.UserFaker.Generate();
            var nonExistentGameId = Guid.NewGuid();

            // Act
            Action act = () => user.RemoveGameFromLibrary(nonExistentGameId);

            // Assert
            act.Should().NotThrow();
        }


        [Fact]
        public void SetPasswordHash_WithValidHash_ShouldSetPasswordHash()
        {
            // Arrange
            var user = _userFaker.Generate();
            var newPasswordHash = "hashedPassword123";

            // Act
            user.SetPasswordHash(newPasswordHash);

            // Assert
            Assert.Equal(newPasswordHash, user.PasswordHash);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void SetPasswordHash_WithInvalidHash_ShouldThrowArgumentException(string invalidHash)
        {
            // Arrange
            var user = _userFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => user.SetPasswordHash(invalidHash));
            Assert.Contains("Password hash não pode ser vazio ou nulo. (Parameter 'passwordHash')", exception.Message);
        }

        [Fact]
        public void SetRole_WithValidRole_ShouldSetRole()
        {
            // Arrange
            var user = _userFaker.Generate();
            var newRole = "Admin";

            // Act
            user.SetRole(newRole);

            // Assert
            Assert.Equal(newRole, user.Role);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void SetRole_WithInvalidRole_ShouldThrowArgumentException(string invalidRole)
        {
            // Arrange
            var user = _userFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => user.SetRole(invalidRole));
            Assert.Contains("Role não pode ser vazio ou nulo. (Parameter 'role')", exception.Message);
        }

        [Fact]
        public void UpdateProfile_WithValidArguments_ShouldUpdateNameAndEmail()
        {
            // Arrange
            var user = _userFaker.Generate();
            var newName = "Updated Name";
            var newEmail = "updated.email@example.com";

            // Act
            user.UpdateProfile(newName, newEmail);

            // Assert
            Assert.Equal(newName, user.Name);
            Assert.Equal(newEmail, user.Email);
        }

        [Theory]
        [InlineData(null, "valid@email.com")]
        [InlineData("", "valid@email.com")]
        [InlineData(" ", "valid@email.com")]
        [InlineData("Valid Name", null)]
        [InlineData("Valid Name", "")]
        [InlineData("Valid Name", " ")]
        [InlineData("Valid Name", "invalid-email")]
        public void UpdateProfile_WithInvalidArguments_ShouldThrowArgumentException(string newName, string newEmail)
        {
            // Arrange
            var user = _userFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => user.UpdateProfile(newName, newEmail));
            Assert.True(exception.Message.Contains("Nome não pode ser vazio ou nulo. (Parameter 'newName')") ||
                        exception.Message.Contains("Email não pode ser vazio ou nulo. (Parameter 'newEmail')") ||
                        exception.Message.Contains("Formato de email é inválido (Parameter 'newEmail')"));
        }
    }
}