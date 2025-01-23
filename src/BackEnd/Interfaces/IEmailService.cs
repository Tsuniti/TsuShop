namespace TsuShopWebApi.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailFromUserToUsAsync(string sendersEmail, string sendersName, string subject, string text);
}