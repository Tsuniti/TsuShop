namespace TsuShopWebApi.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailFromUserToUs(string sendersEmail, string sendersName, string subject, string text);
}