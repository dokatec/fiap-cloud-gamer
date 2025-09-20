﻿using Fcg.Domain.Entities;
using Fcg.Infrastructure.Data;
using Fcg.Infrastructure.Repositories;
using Fcg.Infrastructure.Tests.Fakers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Fcg.Tests.Infrastructure
{
    [Trait("Infrastructure-Repository", "UserRepository")]
    public class UserRepositoryTests : IDisposable
    {
        private readonly FcgDbContext _context;
        private readonly UserRepository _userRepository;

        public UserRepositoryTests()
        {
            // Arrange: Create a fresh in-memory database for each test run.
            var options = new DbContextOptionsBuilder<FcgDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new FcgDbContext(options);
            _userRepository = new UserRepository(_context);
        }

        [Fact]
        public async Task UpdateUserRoleAsync_ShouldUpdateUserRole()
        {
            // Arrange
            var user = EntityFakers.UserFaker.Generate();
            await _userRepository.CreateUserAsync(user);

            var newRole = "SuperAdmin";

            // Act
            // 1. The state of the entity must be changed *before* updating.
            user.SetRole(newRole);

            // 2. The repository persists the changes to the entity.
            await _userRepository.UpdateUserRoleAsync(user.Id, user.Role);

            // Assert
            // 3. Verify by fetching the data directly from the context to ensure it was saved.
            var savedUserEntity = await _context.Users.FindAsync(user.Id);
            savedUserEntity.Should().NotBeNull();
            savedUserEntity!.Role.Should().Be(newRole);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}