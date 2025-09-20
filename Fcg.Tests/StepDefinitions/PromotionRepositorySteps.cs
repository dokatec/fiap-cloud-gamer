using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using Fcg.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Fcg.Tests.StepDefinitions
{
    [Binding]
    public class PromotionRepositorySteps : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly IServiceScope _scope;
        private readonly IPromotionRepository _promotionRepository;
        private readonly FcgDbContext _dbContext;
        private readonly ScenarioContext _scenarioContext;

        public PromotionRepositorySteps(CustomWebApplicationFactory factory, ScenarioContext scenarioContext)
        {
            _scope = factory.Services.CreateScope();
            _promotionRepository = _scope.ServiceProvider.GetRequiredService<IPromotionRepository>();
            _dbContext = _scope.ServiceProvider.GetRequiredService<FcgDbContext>();
            _scenarioContext = scenarioContext;
        }

        public void Dispose()
        {
            _scope.Dispose();
            GC.SuppressFinalize(this);
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _dbContext.Dispose();
        }

        [Given(@"a promotion with name ""(.*)"" and discount (.*) exists in the database")]
        public async Task GivenAPromotionExistsInTheDatabase(string name, decimal discount)
        {
            var promotion = new Fcg.Infrastructure.Tables.Promotion
            {
                Id = Guid.NewGuid(),
                Title = name,
                Description = "Test Description",
                DiscountPercent = discount,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(10),
                Genre = (int)GenreEnum.Acao
            };
            await _dbContext.Promotions.AddAsync(promotion);
            await _dbContext.SaveChangesAsync();
            _scenarioContext.Set(promotion.Id, "PromotionId");
        }

        [Given(@"the following promotions exist in the database:")]
        public async Task GivenTheFollowingPromotionsExistInTheDatabase(Table table)
        {
            var promotionDtos = table.CreateSet<PromotionDto>();
            foreach (var promoDto in promotionDtos)
            {
                var promotion = new Fcg.Infrastructure.Tables.Promotion
                {
                    Id = Guid.NewGuid(),
                    Title = promoDto.Name,
                    Description = "Bulk insert test",
                    DiscountPercent = promoDto.Discount,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(10),
                    Genre = (int)GenreEnum.Acao
                };
                await _dbContext.Promotions.AddAsync(promotion);
            }
            await _dbContext.SaveChangesAsync();
        }

        [When(@"I add a new promotion with name ""(.*)"" and discount (.*)")]
        public async Task WhenIAddANewPromotion(string name, decimal discount)
        {
            var promotion = new Fcg.Infrastructure.Tables.Promotion
            {
                Id = Guid.NewGuid(),
                Title = name,
                Description = "A new promotion",
                DiscountPercent = discount,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Genre = (int)GenreEnum.Aventura
            };
            await _dbContext.Promotions.AddAsync(promotion);
            await _dbContext.SaveChangesAsync();
            _scenarioContext.Set(promotion.Id, "PromotionId");
        }

        [When(@"I retrieve the promotion by its ID")]
        public async Task WhenIRetrieveThePromotionById()
        {
            var promotionId = _scenarioContext.Get<Guid>("PromotionId");
            var promotion = await _dbContext.Promotions.FindAsync(promotionId);
            _scenarioContext.Set(promotion, "RetrievedPromotion");
        }

        [When(@"I try to retrieve a promotion with a random ID")]
        public async Task WhenITryToRetrieveAPromotionWithARandomId()
        {
            var promotion = await _dbContext.Promotions.FindAsync(Guid.NewGuid());
            _scenarioContext.Set(promotion, "RetrievedPromotion");
        }

        [When(@"I retrieve all promotions")]
        public async Task WhenIRetrieveAllPromotions()
        {
            var promotions = _dbContext.Promotions.ToList();
            _scenarioContext.Set(promotions, "AllPromotions");
        }

        [When(@"I delete the promotion")]
        public async Task WhenIDeleteThePromotion()
        {
            var promotionId = _scenarioContext.Get<Guid>("PromotionId");
            var promotion = await _dbContext.Promotions.FindAsync(promotionId);
            if (promotion != null)
            {
                _dbContext.Promotions.Remove(promotion);
                await _dbContext.SaveChangesAsync();
            }
        }

        [Then(@"the promotion should be saved in the database")]
        public async Task ThenThePromotionShouldBeSavedInTheDatabase()
        {
            var promotionId = _scenarioContext.Get<Guid>("PromotionId");
            var promotion = await _dbContext.Promotions.FindAsync(promotionId);
            promotion.Should().NotBeNull();
            _scenarioContext.Set(promotion, "SavedPromotion");
        }

        [Then(@"the saved promotion should have the name ""(.*)"" and discount (.*)")]
        public void ThenTheSavedPromotionShouldHaveTheNameAndDiscount(string name, decimal discount)
        {
            var promotion = _scenarioContext.Get<Fcg.Infrastructure.Tables.Promotion>("SavedPromotion");
            promotion.Title.Should().Be(name);
            promotion.DiscountPercent.Should().Be(discount);
        }

        [Then(@"the retrieved promotion should not be null")]
        public void ThenTheRetrievedPromotionShouldNotBeNull()
        {
            var promotion = _scenarioContext.Get<Fcg.Infrastructure.Tables.Promotion?>("RetrievedPromotion");
            promotion.Should().NotBeNull();
        }

        [Then(@"its name should be ""(.*)""")]
        public void ThenItsNameShouldBe(string name)
        {
            var promotion = _scenarioContext.Get<Fcg.Infrastructure.Tables.Promotion>("RetrievedPromotion");
            promotion.Title.Should().Be(name);
        }

        [Then(@"the result should be null")]
        public void ThenTheResultShouldBeNull()
        {
            var promotion = _scenarioContext.Get<Fcg.Infrastructure.Tables.Promotion?>("RetrievedPromotion");
            promotion.Should().BeNull();
        }

        [Then(@"the result should contain (.*) promotions")]
        public void ThenTheResultShouldContainPromotions(int count)
        {
            var promotions = _scenarioContext.Get<List<Fcg.Infrastructure.Tables.Promotion>>("AllPromotions");
            promotions.Should().HaveCount(count);
        }

        [Then(@"the promotion should no longer exist in the database")]
        public async Task ThenThePromotionShouldNoLongerExistInTheDatabase()
        {
            var promotionId = _scenarioContext.Get<Guid>("PromotionId");
            var promotion = await _dbContext.Promotions.FindAsync(promotionId);
            promotion.Should().BeNull();
        }

        private class PromotionDto
        {
            public string Name { get; set; } = string.Empty;
            public decimal Discount { get; set; }
        }
    }
}
