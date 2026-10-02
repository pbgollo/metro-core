namespace Metro.Domain.Auth
{
    public static class PasswordPolicy
    {
        public const string RequirementsMessage =
            "A senha deve ter no mínimo 8 caracteres, incluindo letras maiúsculas, minúsculas e números.";

        public static bool IsValid(string? password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                return false;
            }

            var hasUpper = false;
            var hasLower = false;
            var hasDigit = false;

            foreach (var character in password)
            {
                if (char.IsUpper(character))
                {
                    hasUpper = true;
                }
                else if (char.IsLower(character))
                {
                    hasLower = true;
                }
                else if (char.IsDigit(character))
                {
                    hasDigit = true;
                }

                if (hasUpper && hasLower && hasDigit)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
