namespace Fcg.Domain.Entities
{
    public class UserGaming
    {
        public Guid Id { get; }
        public User User { get; }
        public Game Game { get; }
        public DateTime PurchasedDate { get; }

        public UserGaming(User user, Game game)
        {
            // validar imediatamente
            User = user ?? throw new ArgumentNullException(nameof(user), "User cannot be null.");
            Game = game ?? throw new ArgumentNullException(nameof(game), "Game cannot be null.");

            Id = Guid.NewGuid();
            PurchasedDate = DateTime.UtcNow; 
        }

        public UserGaming(Guid id, User user, Game game, DateTime purchasedDate)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id não pode ser vazio.", nameof(id));
            User = user ?? throw new ArgumentNullException(nameof(user), "User não pode ser vazio.");
            Game = game ?? throw new ArgumentNullException(nameof(game), "Game não pode ser vazio");
            if (purchasedDate == default)
                throw new ArgumentException("PurchasedDate deve ser preenchida.", nameof(purchasedDate));

            Id = id;
            User = user;
            Game = game;
            PurchasedDate = purchasedDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(purchasedDate, DateTimeKind.Utc)
                : purchasedDate.ToUniversalTime();
        }
    }
}