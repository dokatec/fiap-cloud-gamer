namespace Fcg.Application.Responses
{
    public class CreateUserResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
    }
}
