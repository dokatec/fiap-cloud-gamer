namespace Fcg.Application.Responses
{
    public class CreatePromotionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? PromotionId { get; set; }
    }
}
