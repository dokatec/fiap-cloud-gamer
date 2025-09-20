namespace Fcg.Application.Responses
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public Guid? UserId { get; set; }
        public string? Email { get; set; }
        public string Message { get; set; } = null!;
    }
}
