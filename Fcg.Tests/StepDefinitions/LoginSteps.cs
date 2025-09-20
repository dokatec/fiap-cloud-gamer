using Fcg.Api;
using Fcg.Application.Requests;
using Fcg.Tests;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using TechTalk.SpecFlow;

namespace Fcg.Tests.StepDefinitions
{
    [Binding]
    public class LoginSteps : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private HttpResponseMessage? _response;

        // Helper records to make deserializing responses cleaner
        private record LoginSuccessResponse(string Token, Guid UserId, string Email, string Message);
        private record LoginErrorResponse(string Message);

        public LoginSteps(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Given(@"que um usuário registrado existe com o email ""(.*)"" e a senha ""(.*)""")]
        public async Task GivenARegisteredUserExistsWithEmailAndPassword(string email, string password)
        {
            // We use the user creation endpoint to ensure our test user exists.
            // This makes the test self-contained and independent.
            var createUserRequest = new CreateUserRequest
            {
                Name = "Login Test User",
                Email = email,
                Password = password
            };
            
            var creationResponse = await _client.PostAsJsonAsync("/api/users", createUserRequest);

            // The purpose of this step is to ensure the user exists.
            // A successful creation (2xx) is good.
            // A conflict (409) is also acceptable, as it means the user already exists.
            // Any other error status indicates a problem with the test setup.
            if (!creationResponse.IsSuccessStatusCode && creationResponse.StatusCode != HttpStatusCode.Conflict)
            {
                var errorContent = await creationResponse.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"[GIVEN] Failed to ensure user exists. API returned {creationResponse.StatusCode}. Body: {errorContent}");
            }
        }

        [When(@"eu envio uma requisição de login com o email ""(.*)"" e a senha ""(.*)""")]
        public async Task WhenISendALoginRequestWithEmailAndPassword(string email, string password)
        {
            var loginRequest = new LoginRequest { Email = email, Password = password };
            _response = await _client.PostAsJsonAsync("/api/login", loginRequest);
        }

        [Then(@"a API deve responder com o status '([^']*)'")]
        public void ThenTheAPIShouldRespondWithAnStatus(string expectedStatus)
        {
            // This step is reusable across different feature tests.
            _response.Should().NotBeNull();
            var expectedStatusCode = Enum.Parse<HttpStatusCode>(expectedStatus, true);
            _response.StatusCode.Should().Be(expectedStatusCode);
        }

        [Then(@"a resposta deve conter um token JWT válido")]
        public async Task ThenTheResponseShouldContainAValidJWTToken()
        {
            _response.Should().NotBeNull();
            LoginSuccessResponse? loginResponse = null;
            try
            {
                loginResponse = await _response.Content.ReadFromJsonAsync<LoginSuccessResponse>();
            }
            catch (Exception ex)
            {
                var body = await _response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to deserialize successful login response. Status: {_response.StatusCode}. Body: '{body}'.", ex);
            }

            loginResponse.Should().NotBeNull();
            loginResponse.Token.Should().NotBeNullOrWhiteSpace();
        }

        [Then(@"a resposta da mensagem deve indicar credenciais inválidas")]
        public async Task ThenTheResponseMessageShouldIndicateInvalidCredentials()
        {
            _response.Should().NotBeNull();
            LoginErrorResponse? errorResponse = null;
            try
            {
                errorResponse = await _response.Content.ReadFromJsonAsync<LoginErrorResponse>();
            }
            catch (Exception ex)
            {
                var body = await _response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to deserialize error login response. Status: {_response.StatusCode}. Body: '{body}'.", ex);
            }

            errorResponse.Should().NotBeNull();
            errorResponse.Message.Should().Be("Usuário ou senha inválidos.");
        }
    }
}