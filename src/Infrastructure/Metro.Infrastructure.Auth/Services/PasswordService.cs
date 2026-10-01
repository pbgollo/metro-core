using System.Security.Cryptography;
using System.Text;
using Metro.Domain.Users.Authentication.Services;

namespace Metro.Infrastructure.Auth.Services
{
    public class PasswordService : IPasswordService
    {
        private const string PasswordChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public byte[] GenerateSaltedHash(string value, byte[] salt)
        {
            using var hmac = new HMACSHA256(salt);
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(value));
        }

        public bool ConfirmPassword(byte[] passwordBase, string passwordInput)
        {
            var salt = passwordBase.Take(16).ToArray();
            var hashInput = GenerateSaltedHash(passwordInput, salt);
            var storedHash = passwordBase.Skip(16).ToArray();
            return CryptographicOperations.FixedTimeEquals(storedHash, hashInput);
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

        public byte[] HashPasswordWithSalt(string password)
        {
            var salt = new byte[16];
            RandomNumberGenerator.Fill(salt);

            var hash = GenerateSaltedHash(password, salt);

            var saltedHash = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, saltedHash, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, saltedHash, salt.Length, hash.Length);

            return saltedHash;
        }
    }
}
