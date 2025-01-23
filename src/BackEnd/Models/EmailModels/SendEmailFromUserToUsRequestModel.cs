using System.ComponentModel.DataAnnotations;

namespace TsuShopWebApi.Models.EmailModels;

public class SendEmailFromUserToUsRequestModel
{
    [Required(ErrorMessage = "Sender's email is required to be filled")] 
    public string SendersEmail { get; set; }
    [Required(ErrorMessage = "Sender's name is required to be filled")]
    public string SendersName { get; set; }
    [Required(ErrorMessage = "Subject is required to be filled")]
    public string Subject { get; set; }
    [Required(ErrorMessage = "Email text is required to be filled")] 
    public string Text { get; set; }
}