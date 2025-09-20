using Bogus;
using Fcg.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Fcg.Infrastructure.Tests.Fakers
{
    public static class EntityFakers
    {
        private static readonly object _lock = new object();
        private static Faker<User> _userFaker;
        private static Faker<Game> _gameFaker;
        private static Faker<Promotion> _promotionFaker;

        public static Faker<User> UserFaker
        {
            get
            {
                if (_userFaker == null)
                {
                    lock (_lock)
                    {
                        if (_userFaker == null)
                        {
                            _userFaker = new Faker<User>()
                                // Construtor simples (nome, email, role)
                                .CustomInstantiator(f => new User(
                                    f.Person.FullName,
                                    f.Internet.Email(),
                                    f.PickRandom(new[] { "User", "Admin", "Moderator" })
                                ))
                                // Regras extras para o construtor completo, se necessário
                                .RuleFor(u => u.Id, f => Guid.NewGuid())
                                .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
                                .RuleFor(u => u.Role, f => f.PickRandom(new[] { "User", "Admin", "Moderator" }));
                        }
                    }
                }
                return _userFaker;
            }
        }

        // Se quiser um faker para o construtor completo:
        public static Faker<User> UserFakerFull
        {
            get
            {
                return new Faker<User>()
                    .CustomInstantiator(f => new User(
                        f.Random.Guid(),
                        f.Person.FullName,
                        f.Internet.Email(),
                        f.Internet.Password(),
                        new List<UserGaming>(), // ou gere UserGaming fakes se quiser
                        f.PickRandom(new[] { "User", "Admin", "Moderator" })
                    ));
            }
        }

        public static Faker<Game> GameFaker
        {
            get
            {
                if (_gameFaker == null)
                {
                    lock (_lock)
                    {
                        if (_gameFaker == null)
                        {
                            _gameFaker = new Faker<Game>()
                                .CustomInstantiator(f => new Game(
                                    f.Commerce.ProductName(),
                                    f.Lorem.Sentence(),
                                    f.PickRandom<GenreEnum>(),
                                    f.Finance.Amount(0.99m, 99.99m)
                                ));
                        }
                    }
                }
                return _gameFaker;
            }
        }

        public static Faker<Promotion> PromotionFaker
        {
            get
            {
                if (_promotionFaker == null)
                {
                    lock (_lock)
                    {
                        if (_promotionFaker == null)
                        {
                            _promotionFaker = new Faker<Promotion>()
                                .CustomInstantiator(f =>
                                {
                                    var startDate = f.Date.Past(1, DateTime.UtcNow.AddDays(-30));
                                    var endDate = f.Date.Future(1, startDate.AddDays(60));
                                    var genre = f.PickRandom<GenreEnum>();
                                    return new Promotion(
                                        f.Commerce.ProductName() + " Sale",
                                        f.Lorem.Sentence(),
                                        f.Finance.Amount(5, 50),
                                        startDate,
                                        endDate,
                                        genre
                                    );
                                });
                        }
                    }
                }
                return _promotionFaker;
            }
        }
    }
}