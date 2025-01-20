namespace TsuShopWebApi.Options;

public class EmailOptions
{
    public string SmtpClientHost { get; set; }
    public int SmtpClientPort { get; set; }
    public string Email { get; set; }
    public string AppPassword { get; set; }
    
}