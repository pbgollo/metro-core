namespace Metro.Domain.Users.Authentication.Services
{
    public interface IPasswordService
    {
        byte[] GenerateSaltedHash(string value, byte[] salt);
        bool ConfirmPassword(byte[] passwordBase, string passwordInput);
        string GenerateRandomPassword(int length);
        string GenerateNumericCode(int length);
        string HashCode(string code);
        byte[] HashPasswordWithSalt(string password);
    }
}
