namespace Metro.Domain.Users.Authentication.Services
{
    public interface IPasswordService
    {
        byte[] GenerateSaltedHash(string value, byte[] salt);
        bool ConfirmPassword(byte[] passwordBase, string passwordInput);
        string GenerateRandomPassword(int length);
        byte[] HashPasswordWithSalt(string password);
    }
}
