namespace Fcg.Application.Responses
{
    public class CreateGameResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? GameId { get; set; }
    }
}
