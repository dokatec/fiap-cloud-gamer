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
    [Trait("Application-service", "UserManagementService")]
    public class UserManagementServiceTests : BaseRepositoryTests
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManagementService _userManagementService;

        public UserManagementServiceTests()
        {
            _userRepository = new UserRepository(_context);
            _userManagementService = new UserManagementService(_userRepository);
        }

        [Fact]
        public async Task ChangeUserRoleAsync_ShouldSucceed_WhenActionIsPerformedByAdmin()
        {
            // Arrange
            var adminUser = EntityFakers.UserFaker.RuleFor(u => u.Role, "Admin").Generate();
            var targetUser = EntityFakers.UserFaker.RuleFor(u => u.Role, "Player").Generate();

            await _userRepository.CreateUserAsync(adminUser);
            await _userRepository.CreateUserAsync(targetUser);

            var newRole = "Moderator";

            // Act
            await _userManagementService.ChangeUserRoleAsync(adminUser.Id, targetUser.Id, newRole);

            // Assert
            var updatedTargetUser = await _userRepository.GetUserByIdAsync(targetUser.Id);
            updatedTargetUser.Should().NotBeNull();
            updatedTargetUser.Role.Should().Be(newRole);
        }

        [Fact]
        public async Task ChangeUserRoleAsync_ShouldThrowUnauthorizedAccessException_WhenActionIsPerformedByNonAdmin()
        {
            // Arrange
            var regularUser = EntityFakers.UserFaker.RuleFor(u => u.Role, "Player").Generate();
            var targetUser = EntityFakers.UserFaker.RuleFor(u => u.Role, "Player").Generate();

            await _userRepository.CreateUserAsync(regularUser);
            await _userRepository.CreateUserAsync(targetUser);

            var newRole = "Admin";

            // Act
            Func<Task> act = async () => await _userManagementService.ChangeUserRoleAsync(regularUser.Id, targetUser.Id, newRole);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                     .WithMessage("User is not authorized to perform this action.");
        }

        [Fact]
        public async Task ChangeUserRoleAsync_ShouldThrowInvalidOperationException_WhenTargetUserDoesNotExist()
        {
            // Arrange
            var adminUser = EntityFakers.UserFaker.RuleFor(u => u.Role, "Admin").Generate();
            await _userRepository.CreateUserAsync(adminUser);
            var nonExistentUserId = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await _userManagementService.ChangeUserRoleAsync(adminUser.Id, nonExistentUserId, "Moderator");

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Target user not found.");
        }
    }
}