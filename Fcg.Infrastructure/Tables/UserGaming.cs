namespace Fcg.Infrastructure.Tables
{
    public class UserGaming
    {
        public Guid Id { get; set; }
        public DateTime PurchasedDate { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid GameId { get; set; }
        public Game Game { get; set; } = null!;
    }
}
