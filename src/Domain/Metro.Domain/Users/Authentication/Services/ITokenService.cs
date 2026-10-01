namespace Metro.Domain.Users.Authentication.Services
{
    public interface ITokenService
    {
        public string GenerateToken(Entities.User user);
    }
}
