namespace Fcg.Domain.Entities
{
    public class Promotion
    {
        public Guid Id { get; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public GenreEnum Genre { get; set; }

        public Promotion(string title, string description, decimal discountPercent, DateTime startDate, DateTime endDate, GenreEnum genre)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Título não pode ser vazio ou nulo.", nameof(title));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Descrição Não pode ser vazio ou nulo.", nameof(description));
            if (discountPercent <= 0 || discountPercent > 100)
                throw new ArgumentOutOfRangeException(nameof(discountPercent), "O percemtual de Desconto deve estar entre 0 e 100.");
            if (startDate == default)
                throw new ArgumentException("A data de início deve ser informada.", nameof(startDate));
            if (endDate == default)
                throw new ArgumentException("A data de fim deve ser informada.", nameof(endDate));
            if (startDate >= endDate)
                throw new ArgumentException("A data de início deve ser menor ou igual a data fim.", nameof(startDate));
            if (!Enum.IsDefined(typeof(GenreEnum), genre))
                throw new ArgumentException("O gênero é obrigatório e deve ser válido.", nameof(genre));

            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            DiscountPercent = discountPercent;
            StartDate = startDate.ToUniversalTime(); 
            EndDate = endDate.ToUniversalTime();    
            Genre = genre;
        }

        public Promotion(Guid id, string title, string description, decimal discountPercent, DateTime startDate, DateTime endDate, GenreEnum genre)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id não pode ser vazio.", nameof(id));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title não pode ser vazio.", nameof(title));
            if (discountPercent <= 0 || discountPercent > 100)
                throw new ArgumentOutOfRangeException(nameof(discountPercent), "O percentual de desconto deve estar entre 0 e 100.");
            if (startDate == default)
                throw new ArgumentException("A data início deve ser informada.", nameof(startDate));
            if (endDate == default)
                throw new ArgumentException("A data de fim deve ser informada.", nameof(endDate));
            if (startDate >= endDate)
                throw new ArgumentException("A data de início deve ser menor ou igual a data fim.", nameof(startDate));
            if (!Enum.IsDefined(typeof(GenreEnum), genre))
                throw new ArgumentException("O gênero é obrigatório e deve ser válido.", nameof(genre));

            Id = id;
            Title = title;
            Description = description;
            DiscountPercent = discountPercent;
            StartDate = startDate;
            EndDate = endDate;
            Genre = genre;
        }

        public void UpdatePromotionDetails(string newTitle, string newDescription)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException("Title não pode ser vazio ou nuo.", nameof(newTitle));
            if (string.IsNullOrWhiteSpace(newDescription))
                throw new ArgumentException("Descrição não pode ser vazio.", nameof(newDescription));

            Title = newTitle;
            Description = newDescription;
        }

        public void UpdateGenre(GenreEnum newGenre)
        {
            if (!Enum.IsDefined(typeof(GenreEnum), newGenre))
                throw new ArgumentException("O gênero é obrigatório e deve ser válido.", nameof(newGenre));

            Genre = newGenre;
        }

        public void UpdateDiscount(decimal newDiscountPercent)
        {
            if (newDiscountPercent <= 0 || newDiscountPercent > 100)
                throw new ArgumentOutOfRangeException(nameof(newDiscountPercent), "O percentual de desconto deve estar entre 0 e 100.");

            DiscountPercent = newDiscountPercent;
        }

        public void UpdateDates(DateTime newStartDate, DateTime newEndDate)
        {
            if (newStartDate == default)
                throw new ArgumentException("A data de início deve ser informada.", nameof(newStartDate));
            if (newEndDate == default)
                throw new ArgumentException("A data de fim deve ser informada.", nameof(newEndDate));
            if (newStartDate >= newEndDate)
                throw new ArgumentException("A data de início deve ser menor ou igual a data fim.", nameof(newStartDate));

            StartDate = newStartDate.ToUniversalTime();
            EndDate = newEndDate.ToUniversalTime();
        }
       
        public bool IsActive(DateTime checkDate)
        {
            var utcCheckDate = checkDate.ToUniversalTime();
            return utcCheckDate >= StartDate && utcCheckDate <= EndDate;
        }
    }
}