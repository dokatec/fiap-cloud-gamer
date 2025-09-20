using Fcg.Domain.Entities;
using FluentAssertions;
using System;
using Xunit;

namespace Fcg.Tests.Domain
{
    [Trait("Domain-entity", "Promotion")]
    public class PromotionTests
    {
        [Fact]
        public void Constructor_ShouldThrowArgumentException_WhenEndDateIsBeforeStartDate()
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(-1); // Data de término é anterior à de início

            // Act
            Action act = () => new Promotion(
                Guid.NewGuid(),
                "Invalid Promo",
                "Description",
                10m,
                startDate,
                endDate,
                GenreEnum.Acao
                );
            // Assert
            // A entidade deve validar suas próprias datas para manter a consistência.
            act.Should().Throw<ArgumentException>()
               .WithMessage("A data de início deve ser menor ou igual a data fim.*");
        }

        [Fact]
        public void Constructor_ShouldCreatePromotion_WhenDatesAreValid()
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(1);

            // Act
            var promotion = new Promotion(
                Guid.NewGuid(),
                "Valid Promo",
                "Description",
                15m,
                startDate,
                endDate,
                GenreEnum.Acao
            );
            
            // Assert
            promotion.Should().NotBeNull();
            promotion.StartDate.Should().Be(startDate);
            promotion.EndDate.Should().Be(endDate);
        }

        [Fact]
        public void IsActive_ShouldReturnTrue_WhenCurrentDateIsWithinPromotionPeriod()
        {
            // Arrange
            var promotion = new Promotion(
                Guid.NewGuid(), 
                "Active Promo", 
                "Desc",
                20m,
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(1),
                GenreEnum.Acao
                );


            // Act
            var isActive = (bool)promotion.IsActive(DateTime.UtcNow);

            // Assert
            isActive.Should().BeTrue();
        }

        [Fact]
        public void IsActive_ShouldReturnFalse_WhenCurrentDateIsBeforeStartDate()
        {
            // Arrange
            var promotion = new Promotion(
                Guid.NewGuid(), 
                "Future Promo",
                "Desc",
                20m,
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(2),
                GenreEnum.Acao
                );
            // Act
            var isActive = (bool)promotion.IsActive(DateTime.UtcNow);
            // Assert
            isActive.Should().BeFalse();

        }   


        [Fact]
        public void IsActive_ShouldReturnFalse_WhenCurrentDateIsAfterEndDate()
        {
            // Arrange
            var promotion = new Promotion(
                Guid.NewGuid(), "Expired Promo", "Desc", 20m,
                DateTime.UtcNow.AddDays(-2),
                DateTime.UtcNow.AddDays(-1),
                GenreEnum.Acao
                );
            // Act
            var isActive = (bool)promotion.IsActive(DateTime.UtcNow);

            // Assert
            isActive.Should().BeFalse();
        }
    }
}