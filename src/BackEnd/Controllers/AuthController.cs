using Microsoft.AspNetCore.Mvc;
using TsuShopWebApi.Interfaces;
using TsuShopWebApi.Models;

namespace TsuShopWebApi.Controllers;


[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtGenerator _jwtGenerator;

    public AuthController(IUserService userService, IJwtGenerator jwtGenerator)
    {
        _userService = userService;
        _jwtGenerator = jwtGenerator;
    }

    /// <summary>
    /// Create new user
    /// </summary>
    /// <param name="model">Model with username and password of new user</param>
    /// <response code="200">Success</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="409">User with username already exists</response>
    /// <returns>None</returns>
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Dictionary<string, string[]>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status409Conflict)]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestModel model)
    {
        // Если модель не валидная - возвращаем ошибку со статус кодом 400
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Проверяем, есть ли пользователь с таким логином
        bool usernameExists = await _userService.UsernameExistsAsync(model.Username);

        // Если такое уже есть - ошибка
        if (usernameExists)
            return Conflict(new { error = "Username already exists" });

        await _userService.CreateUserAsync(model.Username, model.Password);

        return Ok();
    }
    
    
    /// <summary>
    /// Login
    /// </summary>
    /// <param name="model">Model with username and password of existing user</param>
    /// <response code="200">Success</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">User with username and password not found</response>
    /// <returns>None</returns>
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Dictionary<string, string[]>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status404NotFound)]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
    {
        // Если модель не валидная - возвращаем ошибку со статус кодом 400
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Ищем айди пользователя по логину и паролю
        Guid? userId = await _userService.GetUserIdByCredentialsAsync(model.Username, model.Password);

        if (userId is null)
            return NotFound(new { error = "User not found" });

        // jwt токен
        var token = await _jwtGenerator.GenerateTokenAsync((Guid)userId);

        return Ok(new { token });
    }
}