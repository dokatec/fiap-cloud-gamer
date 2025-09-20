using Fcg.Domain.Entities;
using Fcg.Infrastructure.Queries;
using Fcg.Infrastructure.Tests.Fakers;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Infrastructure.Tests.Queries
{
    [Trait("Domain-infrastructure", "Promotion Queries")]
    public class PromotionQueryTests : BaseRepositoryTests
    {
        private readonly PromotionQuery _promotionQuery;

        public PromotionQueryTests()
        {
            _promotionQuery = new PromotionQuery(_context);
        }

        [Fact]
        public async Task GetAllPromotionsAsync_ShouldReturnAllPromotions()
        {
            // Arrange
            var promotionsData = EntityFakers.PromotionFaker.Generate(2);
            _context.Promotions.AddRange(promotionsData.Select(p => new Tables.Promotion
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                DiscountPercent = p.DiscountPercent,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Genre = (int)p.Genre
            }));
            await _context.SaveChangesAsync();

            // Act
            var result = await _promotionQuery.GetAllPromotionsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(p => p.Title == promotionsData[0].Title);
            result.Should().Contain(p => p.Title == promotionsData[1].Title);
        }

        [Fact]
        public async Task GetAllPromotionsAsync_ShouldReturnEmptyList_WhenNoPromotionsExist()
        {
            // Arrange
            // No promotions in the database

            // Act
            var result = await _promotionQuery.GetAllPromotionsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByIdPromotionAsync_ShouldReturnPromotion_WhenPromotionExists()
        {
            // Arrange
            var promotionData = EntityFakers.PromotionFaker.Generate();
            _context.Promotions.Add(new Tables.Promotion
            {
                Id = promotionData.Id,
                Title = promotionData.Title,
                Description = promotionData.Description,
                DiscountPercent = promotionData.DiscountPercent,
                StartDate = promotionData.StartDate,
                EndDate = promotionData.EndDate,
                Genre  = (int)promotionData.Genre
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _promotionQuery.GetByIdPromotionAsync(promotionData.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(promotionData.Id);
            result.Title.Should().Be(promotionData.Title);
        }

        [Fact]
        public async Task GetByIdPromotionAsync_ShouldReturnNull_WhenPromotionDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _promotionQuery.GetByIdPromotionAsync(nonExistentId);

            // Assert
            result.Should().BeNull();
        }
    }
}