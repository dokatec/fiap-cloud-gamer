namespace Fcg.Application.Responses
{
    public class BuyGameItemBreakdown
    {
        public Guid GameId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal FinalPrice { get; set; }
        public string? PromotionTitle { get; set; }
    }
}
