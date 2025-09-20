namespace Fcg.Domain.Entities
{
    public class Game
    {
        public Guid Id { get; }
        public string Title { get; private set; } 
        public string Description { get; private set; }
        public GenreEnum Genre { get; private set; }
        public DateTime CreatedAt { get; } 
        public decimal Price { get; private set; }

        public Game(string title, string description, GenreEnum genre, decimal price)
        {            
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Título não pode ser vazio ou nulo", nameof(title));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Descrição não pode ser vazio ou nulo.", nameof(description));
            if (!Enum.IsDefined(typeof(GenreEnum), genre))
                throw new ArgumentException("Gênero inválido.", nameof(genre));
            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Preço não pode ser menor que 0.");

            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Genre = genre;
            Price = price;
            CreatedAt = DateTime.UtcNow; 
        }

        public Game(Guid id, string title, string description, GenreEnum genre, decimal price, DateTime createdAt)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id não pode ser vazio", nameof(id));
            if (string.IsNullOrWhiteSpace(title)) 
                throw new ArgumentException("Título não pode ser vazio ou nulo.", nameof(title));
            if (!Enum.IsDefined(typeof(GenreEnum), genre))
                throw new ArgumentException("Gênero inválido.", nameof(genre));
            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Preço não pode ser menor que 0.");
            

            Id = id;
            Title = title;
            Description = description;
            Genre = genre;
            Price = price;
            CreatedAt = createdAt;
        }
        public void UpdateDetails(string newTitle, string newDescription, GenreEnum newGenre)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException("Título não pode ser vazio ou nulo.", nameof(newTitle));
            if (string.IsNullOrWhiteSpace(newDescription))
                throw new ArgumentException("Descrição não pode ser vazio ou nulo.", nameof(newDescription));
            if (!Enum.IsDefined(typeof(GenreEnum), newGenre))
                throw new ArgumentException("Gênero inválido.", nameof(newGenre));

            Title = newTitle;
            Description = newDescription;
            Genre = newGenre;
        }

        public void ChangePrice(decimal newPrice)
        {
            if (newPrice < 0)
                throw new ArgumentOutOfRangeException(nameof(newPrice), "Preço não pode ser negativo.");

            Price = newPrice;
        }
   }
}