using Fcg.Application.Requests;
using Fcg.Application.Responses;
using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Application.Handlers
{
    public class CreateGameHandler : IRequestHandler<CreateGameRequest, CreateGameResponse>
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<CreateGameHandler> _logger;

        public CreateGameHandler(IGameRepository gameRepository, ILogger<CreateGameHandler> logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public async Task<CreateGameResponse> Handle(CreateGameRequest request, CancellationToken cancellationToken)
        {
            var game = await _gameRepository.GetGameByTitleAsync(request.Title);

            if (game != null)
            {
                _logger.LogWarning($"Tentativa de criar jogo com título já existente: {request.Title}");

                return new CreateGameResponse
                {
                    Success = false,
                    Message = "Jogo já existente"
                };
            }

            game = new Game(request.Title, request.Description, request.Genre, request.Price);

            await _gameRepository.CreateGameAsync(game);

            _logger.LogInformation("Jogo criado com sucesso: {Title}, ID: {Id}", game.Title, game.Id);

            return new CreateGameResponse
            {
                Success = true,
                Message = "Jogo registrado com sucesso.",
                GameId = game.Id
            };
        }
    }
}
