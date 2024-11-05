namespace Common.Interfaces
{
    public interface IEmailService
    {
        Task EnviarEmail(string email, string subject, string message);
    }
}