using Fcg.Domain.Entities;
using Fcg.Infrastructure.Tests.Fakers;
using FluentAssertions;
using System;
using Xunit;

namespace Fcg.Tests.Domain
{
    [Trait("Domain-entity", "User")]
    public class UserTests
    {
        [Fact]
        public void AddGameToLibrary_ShouldAddGame_WhenLibraryIsEmpty()
        {
            // Arrange
            var user = EntityFakers.UserFaker.Generate();
            var game = EntityFakers.GameFaker.Generate();

            // Act
            user.AddGameToLibrary(game);

            // Assert
            user.Library.Should().ContainSingle();
            user.Library.Should().Contain(ug => ug.Game.Id == game.Id);
        }

        [Fact]
        public void AddGameToLibrary_ShouldThrowInvalidOperationException_WhenAddingSameGameTwice()
        {
            // Arrange
            var user = EntityFakers.UserFaker.Generate();
            var game = EntityFakers.GameFaker.Generate();
            user.AddGameToLibrary(game); // Adiciona o jogo pela primeira vez

            // Act
            Action act = () => user.AddGameToLibrary(game); // Tenta adicionar o mesmo jogo novamente

            // Assert
            // A entidade deve proteger a regra de negócio de não adicionar jogos duplicados.
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void RemoveGameFromLibrary_ShouldRemoveGame_WhenGameExists()
        {
            // Arrange
            var user = EntityFakers.UserFaker.Generate();
            var game = EntityFakers.GameFaker.Generate();
            user.AddGameToLibrary(game);
            user.Library.Should().HaveCount(1);

            // Act
            user.RemoveGameFromLibrary(game.Id);

            // Assert
            user.Library.Should().BeEmpty();
        }

        [Fact]
        public void RemoveGameFromLibrary_ShouldNotThrowException_WhenRemovingGameThatIsNotInLibrary()
        {
            // Arrange
            var user = EntityFakers.UserFaker.Generate();
            var gameNotInLibrary = EntityFakers.GameFaker.Generate();

            // Act
            Action act = () => user.RemoveGameFromLibrary(gameNotInLibrary.Id);

            // Assert
            // A operação deve ser idempotente e não quebrar se o item já foi removido ou nunca existiu.
            act.Should().NotThrow();
            user.Library.Should().BeEmpty();
        }

        [Fact]
        public void UpdateProfile_ShouldUpdateNameAndEmail_WithValidData()
        {
            // Arrange
            var user = EntityFakers.UserFaker.Generate();
            var newName = "New Valid Name";
            var newEmail = "new.valid.email@example.com";

            // Act
            user.UpdateProfile(newName, newEmail);

            // Assert
            user.Name.Should().Be(newName);
            user.Email.Should().Be(newEmail);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void UpdateProfile_ShouldThrowArgumentException_WhenNameIsInvalid(string invalidName)
        {
            // Arrange
            var user = EntityFakers.UserFaker.Generate();

            // Act
            Action act = () => user.UpdateProfile(invalidName, "valid@email.com");

            // Assert
            // A entidade deve validar seus próprios dados para manter a consistência.
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void SetRole_ShouldUpdateUserRole_WhenRoleIsValid()
        {
            // Arrange
            var user = new User("Test User", "test@example.com", "User");
            var originalRole = user.Role;
            var newRole = "Admin";

            // Act
            user.SetRole(newRole);

            // Assert
            user.Role.Should().Be(newRole);
            user.Role.Should().NotBe(originalRole);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void SetRole_ShouldThrowArgumentException_WhenRoleIsInvalid(string invalidRole)
        {
            // Arrange
            var user = new User("Test User", "test@example.com", "User");

            // Act
            Action act = () => user.SetRole(invalidRole);

            // Assert
            act.Should().Throw<ArgumentException>();
        }
    }
}