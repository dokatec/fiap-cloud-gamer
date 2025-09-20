using Xunit;
using Fcg.Domain.Entities;
using Bogus;
using System;

namespace Fcg.Tests.UnitTests
{
    [Trait("Domain-Entity", "Promotion")]
    public class PromotionTests
    {
        private readonly Faker<Promotion> _promotionFaker;
        public PromotionTests()
        {
            _promotionFaker = new Faker<Promotion>()
                .CustomInstantiator(f => new Promotion(
                    f.Lorem.Sentence(3),
                    f.Lorem.Paragraph(),
                    f.Random.Decimal(1, 100),
                    f.Date.Past(),
                    f.Date.Future(),
                    GenreEnum.Outro
                ));
        }

        [Fact]
        public void Promotion_Constructor_WithValidArguments_ShouldCreatePromotion()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = DateTime.UtcNow.AddDays(30);
            var promotion = new Promotion(
                "Summer Sale",
                "Great discounts on selected games!",
                50,
                startDate,
                endDate,
                GenreEnum.Outro
            );

            // Act & Assert
            Assert.NotEqual(Guid.Empty, promotion.Id);
            Assert.False(string.IsNullOrWhiteSpace(promotion.Title));
            Assert.False(string.IsNullOrWhiteSpace(promotion.Description));
            Assert.True(promotion.DiscountPercent > 0 && promotion.DiscountPercent <= 100);
            Assert.True(promotion.StartDate.Kind == DateTimeKind.Utc);
            Assert.True(promotion.EndDate.Kind == DateTimeKind.Utc);
            Assert.True(promotion.StartDate < promotion.EndDate);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Promotion_Constructor_WithInvalidTitle_ShouldThrowArgumentException(string invalidTitle)
        {
            // Arrange
            var faker = _promotionFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Promotion(
                invalidTitle,
                faker.Description,
                faker.DiscountPercent,
                faker.StartDate,
                faker.StartDate.AddDays(1),
                GenreEnum.Outro
            ));
            Assert.Contains("Título não pode ser vazio ou nulo.", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Promotion_Constructor_WithInvalidDescription_ShouldThrowArgumentException(string invalidDescription)
        {
            // Arrange
            var faker = _promotionFaker.Generate();
            invalidDescription = "";
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Promotion(
                faker.Title,
                invalidDescription,
                faker.DiscountPercent,
                faker.StartDate.AddDays(-1),
                faker.StartDate.AddDays(1),
                GenreEnum.Outro
            ));
            Assert.True(exception.Message.Contains("Descrição Não pode ser vazio ou nulo.") == true);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        [InlineData(-10)]
        public void Promotion_Constructor_WithInvalidDiscountPercent_ShouldThrowArgumentOutOfRangeException(decimal invalidDiscount)
        {
            // Arrange
            var faker = _promotionFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Promotion(
                faker.Title,
                faker.Description,
                invalidDiscount,
                faker.StartDate,
                faker.EndDate,
                GenreEnum.Outro
            ));
            Assert.True(exception.Message.Contains("O percemtual de Desconto deve estar entre 0 e 100.") == true);
        }

        [Fact]
        public void Promotion_Constructor_WithStartDateAfterEndDate_ShouldThrowArgumentException()
        {
            // Arrange
            var faker = _promotionFaker.Generate();
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(-10);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Promotion(
                faker.Title,
                faker.Description,
                faker.DiscountPercent,
                startDate,
                endDate,
                GenreEnum.Outro
            ));
            Assert.Contains("A data de início deve ser menor ou igual a data fim.", exception.Message);
        }

        [Fact]
        public void UpdatePromotionDetails_WithValidArguments_ShouldUpdateProperties()
        {
            // Arrange
            var promotion = _promotionFaker.Generate();
            var newTitle = "New Promotion Title";
            var newDescription = "Updated description for the promotion.";

            // Act
            promotion.UpdatePromotionDetails(newTitle, newDescription);

            // Assert
            Assert.Equal(newTitle, promotion.Title);
            Assert.Equal(newDescription, promotion.Description);
        }

        [Fact]
        public void UpdateDiscount_WithValidDiscount_ShouldUpdateDiscountPercent()
        {
            // Arrange
            var promotion = _promotionFaker.Generate();
            var newDiscount = 25m;

            // Act
            promotion.UpdateDiscount(newDiscount);

            // Assert
            Assert.Equal(newDiscount, promotion.DiscountPercent);
        }

        [Fact]
        public void UpdateDiscount_WithInvalidDiscount_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var promotion = _promotionFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => promotion.UpdateDiscount(101m));
            Assert.Contains("percentual de desconto deve estar entre 0 e 100.", exception.Message);
        }

        [Fact]
        public void UpdateDates_WithValidDates_ShouldUpdateDates()
        {
            // Arrange
            var promotion = _promotionFaker.Generate();
            var newStartDate = DateTime.UtcNow.AddMonths(1);
            var newEndDate = DateTime.UtcNow.AddMonths(2);

            // Act
            promotion.UpdateDates(newStartDate, newEndDate);

            // Assert
            Assert.Equal(newStartDate.ToUniversalTime(), promotion.StartDate);
            Assert.Equal(newEndDate.ToUniversalTime(), promotion.EndDate);
        }

        [Fact]
        public void UpdateDates_WithInvalidDates_ShouldThrowArgumentException()
        {
            // Arrange
            var promotion = _promotionFaker.Generate();
            var newStartDate = DateTime.UtcNow;
            var newEndDate = newStartDate.AddDays(-10);


            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => promotion.UpdateDates(newStartDate, newEndDate));
            Assert.Contains("A data de início deve ser menor ou igual a data fim.", exception.Message);
        }

        [Fact]
        public void IsActive_WhenPromotionIsActive_ShouldReturnTrue()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-5);
            var endDate = DateTime.UtcNow.AddDays(5);
            var promotion = new Promotion("Active Promo", "Active", 10, startDate, endDate, GenreEnum.Outro);

            // Act
            var isActive = promotion.IsActive(DateTime.UtcNow);

            // Assert
            Assert.True(isActive);
        }

        [Fact]
        public void IsActive_WhenPromotionIsInactiveBeforeStartDate_ShouldReturnFalse()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(5);
            var endDate = DateTime.UtcNow.AddDays(10);
            var promotion = new Promotion("Future Promo", "Future", 10, startDate, endDate, GenreEnum.Outro);

            // Act
            var isActive = promotion.IsActive(DateTime.UtcNow);

            // Assert
            Assert.False(isActive);
        }

        [Fact]
        public void IsActive_WhenPromotionIsInactiveAfterEndDate_ShouldReturnFalse()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-10);
            var endDate = DateTime.UtcNow.AddDays(-5);
            var promotion = new Promotion("Past Promo", "Past", 10, startDate, endDate, GenreEnum.Outro);

            // Act
            var isActive = promotion.IsActive(DateTime.UtcNow);

            // Assert
            Assert.False(isActive);
        }
    }
}