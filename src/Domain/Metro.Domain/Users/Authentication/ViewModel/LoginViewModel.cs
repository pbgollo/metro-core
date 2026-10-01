namespace Metro.Domain.Users.Authentication.ViewModel
{
    public class LoginViewModel
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
    }
}
