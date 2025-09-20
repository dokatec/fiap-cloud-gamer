using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using Fcg.Domain.Services;
using Fcg.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Fcg.Tests.StepDefinitions
{
    [Binding]
    public class PromotionSteps : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceScope _scope;

        // Serviços resolvidos do escopo para preparar o teste
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly FcgDbContext _dbContext;

        // Estado para o cenário
        private HttpResponseMessage _response;
        private Guid _promotionId;

        public PromotionSteps(WebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateClient();

            // Cria um novo escopo de injeção de dependência para cada cenário, garantindo isolamento
            _scope = factory.Services.CreateScope();

            // Resolve os serviços a partir do escopo
            _userRepository = _scope.ServiceProvider.GetRequiredService<IUserRepository>();
            _jwtTokenService = _scope.ServiceProvider.GetRequiredService<IJwtTokenService>();
            _passwordHasherService = _scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();
            _dbContext = _scope.ServiceProvider.GetRequiredService<FcgDbContext>();
        }

        // Garante que os recursos do escopo sejam liberados após cada cenário
        public void Dispose() => _scope.Dispose();

        [Given(@"que a base de dados está limpa")]
        public void GivenQueABaseDeDadosEstaLimpa()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }

        [Given(@"que existe uma promoção com o nome ""(.*)""")]
        public async Task GivenQueExisteUmaPromocaoComONome(string name)
        {
            var promotion = new Fcg.Infrastructure.Tables.Promotion
            {
                Id = Guid.NewGuid(),
                Title = name,
                Description = "Promoção de teste",
                DiscountPercent = 10,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7),
                Genre = (int)GenreEnum.Acao
            };
            await _dbContext.Promotions.AddAsync(promotion);
            await _dbContext.SaveChangesAsync();
            _promotionId = promotion.Id; // Guarda o ID para usar no step 'When'
        }

        [Given(@"eu estou autenticado como um ""(.*)""")]
        public async Task GivenEuEstouAutenticadoComoUm(string role)
        {
            var user = new User($"{role} User", $"{role.ToLower()}@test.com", role);
            user.SetPasswordHash(_passwordHasherService.Hash("Password@123"));
            await _userRepository.CreateUserAsync(user);

            var token = _jwtTokenService.GenerateToken(user.Email, user.Role);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        [When(@"eu envio uma requisição DELETE para o endpoint de promoções")]
        public async Task WhenEuEnvioUmaRequisicaoDeleteParaOEndpointDePromocoes()
        {
            _response = await _httpClient.DeleteAsync($"/api/promotions/{_promotionId}");
        }

        [When(@"eu envio uma requisição DELETE para o endpoint de promoções com o ID ""(.*)""")]
        public async Task WhenEuEnvioUmaRequisicaoDeleteParaOEndpointDePromocoesComOID(string id)
        {
            _response = await _httpClient.DeleteAsync($"/api/promotions/{id}");
        }

        [Then(@"o status da resposta deve ser (.*)")]
        public void ThenOStatusDaRespostaDeveSer(int statusCode)
        {
            _response.StatusCode.Should().Be((HttpStatusCode)statusCode);
        }

        [Then(@"a promoção não deve mais existir no banco de dados")]
        public async Task ThenAPromocaoNaoDeveMaisExistirNoBancoDeDados()
        {
            var promotion = await _dbContext.Promotions.FindAsync(_promotionId);
            promotion.Should().BeNull();
        }
    }
}