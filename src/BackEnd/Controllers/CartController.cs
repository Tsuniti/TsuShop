using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TsuShopWebApi.Interfaces;
using TsuShopWebApi.Models.CartModels;

namespace TsuShopWebApi.Controllers;

[Route("cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    /// <summary>
    /// Get all cart items
    /// </summary>
    /// <response code="200">Success</response>
    /// <response code="500">Internal server error</response>
    /// <returns>List of cart items</returns>
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<IActionResult> GetAllCartItemsAsync()
    {
        var cartItems = await _cartService.GetAllCartItemsAsync(UserId);
        if (cartItems is null)
        {
            return StatusCode(500,
                new
                {
                    message =
                        $"An error occurred while fetching cart items. Most likely the problem is with UserId: {UserId}"
                });
        }

        return Ok(cartItems);
    }


    /// <summary>
    /// Update todo
    /// </summary>
    /// <param name="model">Model with todo id, new title, new status(isCompleted)</param>
    /// <response code="200">Success</response>
    /// <response code="400">Quantity not changed</response>
    /// <returns>string</returns>
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Dictionary<string, string[]>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status404NotFound)]
    [HttpPut]
    public async Task<IActionResult> ChangeQuantityAsync([FromBody] ChangeQuantityRequestModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _cartService.ChangeQuantityAsync(model.ProductId, model.Quantity, UserId))
            return BadRequest("Probably product quantity is less than you trying to add");

        return Ok("Quantity changed");
    }


    /// <summary>
    /// Add in cart one or more of the same item
    /// </summary>
    /// <param name="model">Model with text of comment and todo id</param>
    /// <response code="200">Success</response>
    /// <response code="400">Invalid request data</response>
    /// <returns>string</returns>
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Dictionary<string, string[]>), StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<IActionResult> AddSomeInCartAsync([FromBody] AddSomeInCartRequestModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _cartService.AddSomeInCartAsync(model.ProductId, model.Quantity, UserId);

        return Ok("Item added or quantity increased");
    }
    
    
}