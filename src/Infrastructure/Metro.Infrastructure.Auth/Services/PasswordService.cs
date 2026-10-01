using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Metro.Domain.Users.Authentication.Services;

namespace Metro.Infrastructure.Auth.Services
{
    public class PasswordService : IPasswordService
    {
        private const string PasswordChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private readonly PasswordHasher<object> _passwordHasher = new();

        public string HashPassword(string password)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(password);
            return _passwordHasher.HashPassword(null!, password);
        }

        public bool ConfirmPassword(string passwordHash, string passwordInput)
        {
            if (string.IsNullOrWhiteSpace(passwordHash) || string.IsNullOrWhiteSpace(passwordInput))
            {
                return false;
            }

            var result = _passwordHasher.VerifyHashedPassword(null!, passwordHash, passwordInput);
            return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
        }

        public string GenerateRandomPassword(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "O comprimento da senha deve ser maior que zero.");
            }

            var password = new char[length];
            for (var i = 0; i < length; i++)
            {
                password[i] = PasswordChars[RandomNumberGenerator.GetInt32(PasswordChars.Length)];
            }

            return new string(password);
        }

        public string GenerateNumericCode(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "O comprimento do código deve ser maior que zero.");
            }

            var code = new char[length];
            for (var i = 0; i < length; i++)
            {
                code[i] = (char)('0' + RandomNumberGenerator.GetInt32(10));
            }

            return new string(code);
        }

        public string HashCode(string code)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(code));
            return Convert.ToHexString(hash);
        }
    }
}
