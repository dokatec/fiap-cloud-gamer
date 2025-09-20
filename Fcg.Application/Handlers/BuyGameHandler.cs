using Fcg.Application.Requests;
using Fcg.Application.Responses;
using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Application.Handlers
{
    public class BuyGameHandler : IRequestHandler<BuyGameRequest, BuyGameResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly ILogger<BuyGameHandler> _logger;

        public BuyGameHandler(
            IUserRepository userRepository,
            IGameRepository gameRepository,
            IPromotionRepository promotionRepository,
            ILogger<BuyGameHandler> logger)
        {
            _userRepository = userRepository;
            _gameRepository = gameRepository;
            _promotionRepository = promotionRepository;
            _logger = logger;
        }

        public async Task<BuyGameResponse> Handle(BuyGameRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.UserId);

            if (user is null)
            {
                _logger.LogWarning("Usuário com Id {UserId} não encontrado!", request.UserId);
                return new BuyGameResponse
                {
                    Success = false,
                    Message = "Usuário não encontrado!"
                };
            }

            var games = await _gameRepository.GetGamesByIdsAsync(request.GamesIds);
            var gamesList = games?.ToList() ?? new List<Game>();

            if (gamesList.Count == 0 || gamesList.Count != request.GamesIds.Count())
            {
                _logger.LogWarning("Alguns jogos não foram encontrados! Solicitados: {Requested}, Encontrados: {Found}",
                    request.GamesIds.Count(), gamesList.Count);

                return new BuyGameResponse
                {
                    Success = false,
                    Message = "Alguns jogos não foram encontrados!"
                };
            }

            var gamesAlreadyOwned = gamesList
                .Where(game => user.Library.Any(ug => ug.Game.Id == game.Id))
                .Select(game => game.Title)
                .ToList();

            if (gamesAlreadyOwned.Count != 0)
            {
                var message = $"O usuário já possui os seguintes jogos na biblioteca: {string.Join(", ", gamesAlreadyOwned)}";
                _logger.LogWarning(message);
                return new BuyGameResponse
                {
                    Success = false,
                    Message = message
                };
            }

            var promotions = (await _promotionRepository.GetValidPromotionsAsync()).ToList();

            decimal totalWithDiscount = 0m;
            var items = new List<BuyGameItemBreakdown>();

            foreach (var game in gamesList)
            {
                decimal finalPrice = game.Price;

                var matchingPromotion = promotions
                    .Where(p => p.Genre == game.Genre)
                    .OrderByDescending(p => p.DiscountPercent)
                    .FirstOrDefault();

                if (matchingPromotion == null)
                {
                    matchingPromotion = promotions
                        .Where(p => p.Genre == GenreEnum.Outro)
                        .OrderByDescending(p => p.DiscountPercent)
                        .FirstOrDefault();
                }

                if (matchingPromotion != null && matchingPromotion.DiscountPercent > 0)
                {
                    var discount = game.Price * (matchingPromotion.DiscountPercent / 100m);
                    finalPrice = Math.Max(0, game.Price - discount);

                    _logger.LogInformation(
                        "Desconto de {DiscountPercent}% aplicado ao jogo {Title} (gênero: {Genre}). Valor final: {FinalPrice}",
                        matchingPromotion.DiscountPercent, game.Title, game.Genre, finalPrice
                    );
                }

                totalWithDiscount += finalPrice;

                user.AddGameToLibrary(game);
            }

            await _userRepository.UpdateUserLibraryAsync(user);

            return new BuyGameResponse
            {
                Success = true,
                Message = "Jogos comprados com sucesso!",
                GamesIds = request.GamesIds,
                UserId = user.Id,
                TotalPriceWithDiscount = totalWithDiscount
            };
        }
    }
}