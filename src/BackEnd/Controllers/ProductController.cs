using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TsuShopWebApi.Interfaces;
using TsuShopWebApi.Models.ProductModels;

namespace TsuShopWebApi.Controllers;

[Route("product")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


    public ProductController(IProductService productService)
    {
        _productService = productService;
    }


    /// <summary>
    /// Get some products
    /// </summary>
    /// <param name="page">int</param>
    /// <param name="quantity">int</param>
    /// <param name="minPrice">int</param>
    /// <param name="maxPrice">int</param>
    /// <param name="sortBy">string</param>
    /// <param name="isAscending">bool</param>
    /// <param name="category">string</param>
    /// <param name="name">string</param>
    /// <returns>list of products</returns>
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int quantity = 21,
        [FromQuery] int minPrice = 0,
        [FromQuery] int maxPrice = Int32.MaxValue,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool isAscending = true,
        [FromQuery] string? category = null,
        [FromQuery] string? name = null)
    {
        var products =
            await _productService.GetSomeAsync(
                page,
                quantity,
                minPrice,
                maxPrice,
                sortBy,
                isAscending,
                category,
                name);

        return Ok(products);
    }

    /// <summary>
    /// Get max price
    /// </summary>
    /// <returns>double</returns>
    [HttpGet("max-price")]
    public async Task<IActionResult> GetMaxPrice()
    {
        return Ok(await _productService.GetMaxPrice());
    }

    
    /// <summary>
    /// Get all product categories
    /// </summary>
    /// <returns>ICollection of strings</returns>
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        return Ok(await _productService.GetCategories());
    }

    /// <summary>
    /// Get one product by id
    /// </summary>
    /// <param name="id">Product's guid</param>
    /// <returns>product</returns>
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById([FromRoute] Guid id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = await _productService.GetByIdAsync(id);

        return Ok(product);
    }


    /// <summary>
    /// Create new product
    /// </summary>
    /// <param name="model">Model with (string)Name, (string)Description, (string)Category, (double)Price, (int)Quantity</param>
    /// <returns>product</returns>
    [Authorize]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("create")]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequestModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product =
            await _productService.CreateAsync(
                model.Name,
                model.Image,
                model.Description,
                model.Category,
                model.Price,
                model.Quantity,
                UserId);

        if (product is null)
            return BadRequest("Product not created");

        return Ok(product);
    }

    /// <summary>
    /// Update product
    /// </summary>
    /// <param name="model">Model with (Guid)ProductId, (string)Name, (string)Description, (string)Category, (double)Price, (int)Quantity</param>
    /// <returns>product</returns>
    [Authorize]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut]
    public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductRequestModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product =
            await _productService.UpdateAsync(
                model.ProductId,
                model.Name,
                model.Image,
                model.Description,
                model.Category,
                model.Price,
                model.Quantity,
                UserId);

        if (product is null)
            return BadRequest("Product not updated");

        return Ok(product);
    }

    
    /// <summary>
    /// Delete one product
    /// </summary>
    /// <param name="model">model with (Guid)ProductId</param>
    /// <returns>string</returns>
    [Authorize]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete]
    public async Task<IActionResult> DeleteProduct([FromBody] DeleteProductRequestModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _productService.DeleteAsync(model.ProductId, UserId))
        {
            return BadRequest("Product not updated");
        }
        return Ok("Product deleted");
    }
    
    /// <summary>
    /// Remove some items of product from stock
    /// </summary>
    /// <param name="model">model with (Guid)ProductId, (int)Quantity</param>
    /// <returns>string</returns>
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("remove-from-stock")]
    public async Task<IActionResult> RemoveSomeFromStock([FromBody] RemoveSomeFromStockRequestModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _productService.RemoveSomeFromStockAsync(model.ProductId, model.Quantity))
        {
            return BadRequest("Items not removed");
        }
        return Ok("Items removed");
    }
    
    /// <summary>
    /// Add some items of product from stock
    /// </summary>
    /// <param name="model">model with (Guid)ProductId, (int)Quantity</param>
    /// <returns>string</returns>
    [Authorize]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("add-to-stock")]
    public async Task<IActionResult> AddSomeToStock([FromBody] RemoveSomeFromStockRequestModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _productService.AddSomeToStockAsync(model.ProductId, model.Quantity, UserId))
        {
            return BadRequest("Items not added");
        }
        return Ok("Items added");
    }
    
}