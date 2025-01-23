using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TsuShopWebApi.Interfaces;
using TsuShopWebApi.Models.EmailModels;

namespace TsuShopWebApi.Controllers;


[Route("email")]
[Authorize]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    /// <summary>
    /// Send a letter to our email
    /// </summary>
    /// <param name="model">Model with senders email, senders name, subject, text</param>
    /// <response code="200">Success</response>
    /// <response code="400">Invalid request data</response>
    /// <returns>string</returns>
    public async Task<IActionResult> SendEmailFromUserToUs([FromBody] SendEmailFromUserToUsRequestModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        await _emailService.SendEmailFromUserToUsAsync(model.SendersEmail, model.SendersName, model.Subject, model.Text);
        return Ok("Email sended");
    }
}