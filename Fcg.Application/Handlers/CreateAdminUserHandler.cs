using Fcg.Application.Requests;
using Fcg.Application.Responses;
using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using Fcg.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Application.Handlers
{
    public class CreateAdminUserHandler : IRequestHandler<CreateAdminUserRequest, CreateAdminUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly ILogger<CreateAdminUserHandler> _logger;

        public CreateAdminUserHandler(IUserRepository userRepository, IPasswordHasherService passwordHasherService, ILogger<CreateAdminUserHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
            _logger = logger;
        }

        public async Task<CreateAdminUserResponse> Handle(CreateAdminUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user != null)
            {
                _logger.LogWarning($"Tentativa de criar usuário com e-mail já existente: {request.Email}");

                return new CreateAdminUserResponse
                {
                    Success = false,
                    Message = "E-mail já cadastrado."
                };
            }

            user = new User(request.Name, request.Email, "Admin");
            user.SetPasswordHash(_passwordHasherService.Hash(request.Password));

            await _userRepository.CreateUserAsync(user);

            _logger.LogInformation("Usuário administrador criado com sucesso: {Email}, ID: {Id}", user.Email, user.Id);

            return new CreateAdminUserResponse
            {
                Success = true,
                Message = "Usuário administrador registrado com sucesso.",
                UserId = user.Id
            };
        }
    }
}