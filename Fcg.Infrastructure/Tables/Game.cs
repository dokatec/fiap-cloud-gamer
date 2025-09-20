using Fcg.Domain.Entities;

namespace Fcg.Infrastructure.Tables
{
    public class Game
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Genre { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Price { get; set; }
    }
}
