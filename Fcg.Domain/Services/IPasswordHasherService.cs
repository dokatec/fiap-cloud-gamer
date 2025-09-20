namespace Fcg.Domain.Services
{
    public interface IPasswordHasherService
    {
        string Hash(string senha);
        bool Verify(string senha, string hash);
    }
}
