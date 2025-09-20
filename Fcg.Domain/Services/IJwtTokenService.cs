namespace Fcg.Domain.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(string email, string role);
    }
}
