using System.Net;
using System.Net.Mail;
using TsuShopWebApi.Interfaces;
using TsuShopWebApi.Options;

namespace TsuShopWebApi.Services;

public class EmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;

    public EmailService(EmailOptions emailOptions)
    {
        _emailOptions = emailOptions;
    }

    public async Task<bool> SendEmailFromUserToUsAsync(string sendersEmail, string sendersName, string subject, string text)
    {
        if (string.IsNullOrEmpty(sendersEmail) ||
            string.IsNullOrEmpty(sendersName) ||
            string.IsNullOrEmpty(subject) ||
            string.IsNullOrEmpty(text))
        {
            return false;
        }

        using (var client = new SmtpClient(_emailOptions.SmtpClientHost, _emailOptions.SmtpClientPort))
        {
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_emailOptions.Email, _emailOptions.AppPassword);

            string body = $"Senders name: {sendersName}\n" +
                          $"Senders email: {sendersEmail}\n" +
                          $"\n" +
                          text;

            var message = new MailMessage(
                _emailOptions.Email,
                _emailOptions.Email,
                $"(Contact Us): {subject}",
                body
            );
            message.IsBodyHtml = false;


            try
            {
                await client.SendMailAsync(message);

                ////// send result to user

                body = $"Dear {sendersName},\n" +
                       $"\n" +
                       $"We have received your letter. We will try to answer as soon as possible.\n" +
                       $"\n" +
                       $"Please do not reply to this email, we will contact you ourselves.";

                message = new MailMessage(
                    _emailOptions.Email,
                    sendersEmail,
                    $"We have received your letter",
                    body
                );
                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send email error: {ex.Message}");
                return false;
            }
        }
    }
}