namespace Metro.Domain.Services
{
    public interface IEmailService
    {
        Task Send(string to, string subject, string html);
    }
}
