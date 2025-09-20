using Fcg.Application.Requests;
using Fcg.Application.Responses;
using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using Fcg.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Application.Handlers
{
    public class CreateUserHandler : IRequestHandler<CreateUserRequest, CreateUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly ILogger<CreateUserHandler> _logger;

        public CreateUserHandler(IUserRepository userRepository, IPasswordHasherService passwordHasherService, ILogger<CreateUserHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
            _logger = logger;
        }

        public async Task<CreateUserResponse> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user != null)
            {
                _logger.LogWarning($"Tentativa de criar usuário com e-mail já existente: {request.Email}");

                return new CreateUserResponse
                {
                    Success = false,
                    Message = "E-mail já cadastrado."
                };
            }

            user = new User(request.Name, request.Email);
            user.SetPasswordHash(_passwordHasherService.Hash(request.Password));

            await _userRepository.CreateUserAsync(user);

            _logger.LogInformation("Usuário criado com sucesso: {Email}, ID: {Id}", user.Email, user.Id);

            return new CreateUserResponse
            {
                Success = true,
                Message = "Usuário registrado com sucesso.",
                UserId = user.Id
            };
        }
    }
}
