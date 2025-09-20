using Fcg.Domain.Entities;
using Fcg.Infrastructure.Repositories;
using Fcg.Infrastructure.Tests.Fakers;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Infrastructure.Tests
{
    public class UserRepositoryTests : BaseRepositoryTests
    {
        private readonly UserRepository _userRepository;

        public UserRepositoryTests()
        {
            _userRepository = new UserRepository(_context);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            var user = EntityFakers.UserFaker.Generate();

            // Act
            var userId = await _userRepository.CreateUserAsync(user);

            // Assert
            userId.Should().NotBeEmpty();
            var savedUserEntity = await _context.Users.FindAsync(userId);
            savedUserEntity.Should().NotBeNull();
            savedUserEntity.Name.Should().Be(user.Name);
            savedUserEntity.Email.Should().Be(user.Email);
            savedUserEntity.Role.Should().Be(user.Role);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = EntityFakers.UserFaker.Generate();
            await _userRepository.CreateUserAsync(user);

            // Act
            var retrievedUser = await _userRepository.GetUserByEmailAsync(user.Email);

            // Assert
            retrievedUser.Should().NotBeNull();
            retrievedUser.Id.Should().Be(user.Id);
            retrievedUser.Name.Should().Be(user.Name);
            retrievedUser.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentEmail = "nonexistent@example.com";

            // Act
            var retrievedUser = await _userRepository.GetUserByEmailAsync(nonExistentEmail);

            // Assert
            retrievedUser.Should().BeNull();
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = EntityFakers.UserFaker.Generate();
            await _userRepository.CreateUserAsync(user);

            // Act
            var retrievedUser = await _userRepository.GetUserByIdAsync(user.Id);

            // Assert
            retrievedUser.Should().NotBeNull();
            retrievedUser.Id.Should().Be(user.Id);
            retrievedUser.Name.Should().Be(user.Name);
            retrievedUser.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var retrievedUser = await _userRepository.GetUserByIdAsync(nonExistentId);

            // Assert
            retrievedUser.Should().BeNull();
        }

        [Fact]
        public async Task UpdateUserProfileAsync_ShouldUpdateUserNameAndEmail()
        {
            // Arrange
            var user = EntityFakers.UserFaker.Generate();
            await _userRepository.CreateUserAsync(user);

            var updatedUser = await _userRepository.GetUserByIdAsync(user.Id);
            updatedUser.Should().NotBeNull();

            var newName = "Updated Name";
            var newEmail = "updated@example.com";
            updatedUser.UpdateProfile(newName, newEmail);

            // Act
            await _userRepository.UpdateUserProfileAsync(updatedUser);

            // Assert
            var savedUser = await _userRepository.GetUserByIdAsync(user.Id);
            savedUser.Should().NotBeNull();
            savedUser.Name.Should().Be(newName);
            savedUser.Email.Should().Be(newEmail);
        }

        
        

        
    }
}