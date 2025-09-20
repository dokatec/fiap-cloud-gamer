using Fcg.Application.Requests;
using Fcg.Application.Responses;
using Fcg.Domain.Repositories;
using Fcg.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Application.Handlers
{
    public class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<LoginHandler> _logger;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginHandler(IUserRepository userRepository, IPasswordHasherService passwordHasherService, ILogger<LoginHandler> logger, IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null || !_passwordHasherService.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Tentativa de login inválida para o e-mail: {Email}", request.Email);

                return new LoginResponse
                {
                    Success = false,
                    Message = "Credenciais inválidas."
                };
            }

            var token = _jwtTokenService.GenerateToken(user.Email, user.Role);

            _logger.LogInformation("Usuário {Email} logado com sucesso.", user.Email);

            return new LoginResponse
            {
                Success = true,
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Message = "Login realizado com sucesso!"
            };
        }
    }
}
