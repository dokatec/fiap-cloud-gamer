using Fcg.Infrastructure.Repositories;
using Fcg.Infrastructure.Tests.Fakers;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Infrastructure.Tests
{
    [Trait("Domain-infrastructure", "Promotion Repository")]
    public class PromotionRepositoryTests : BaseRepositoryTests
    {
        private readonly PromotionRepository _promotionRepository;

        public PromotionRepositoryTests()
        {
            // Supondo que PromotionRepository exista e siga o mesmo padrão.
            _promotionRepository = new PromotionRepository(_context);
        }

        [Fact]
        public async Task UpdatePromotionAsync_ShouldFailSilently_WhenPromotionDoesNotExist()
        {
            // Arrange
            var nonExistentPromotion = EntityFakers.PromotionFaker.Generate();

            // Act
            // Supondo que exista um método UpdateAsync no repositório.
            Func<Task> act = async () => await _promotionRepository.UpdatePromotionAsync(nonExistentPromotion);

            // Assert
            await act.Should().NotThrowAsync();
        }
    }
}