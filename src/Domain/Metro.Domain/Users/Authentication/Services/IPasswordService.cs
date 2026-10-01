namespace Metro.Domain.Users.Authentication.Services
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool ConfirmPassword(string passwordHash, string passwordInput);
        string GenerateRandomPassword(int length);
        string GenerateNumericCode(int length);
        string HashCode(string code);
    }
}
