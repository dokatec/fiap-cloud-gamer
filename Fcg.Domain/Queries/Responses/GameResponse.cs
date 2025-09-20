using Fcg.Domain.Entities;

namespace Fcg.Domain.Queries.Responses
{
    public class GameResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public GenreEnum Genre { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Price { get; set; }
    }
}