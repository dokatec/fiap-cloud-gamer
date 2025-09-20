namespace Fcg.Application.Responses
{
    public class BuyGameResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public IEnumerable<Guid> GamesIds { get; set; } = Array.Empty<Guid>();
        public decimal? TotalPriceWithDiscount { get; set; }
    }
}
