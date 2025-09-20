using Fcg.Domain.Entities;

namespace Fcg.Infrastructure.Tables
{
    public class Promotion
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Genre { get; set; }
    }
}
