using Microsoft.EntityFrameworkCore;
using TsuShopWebApi.Entities;
using TsuShopWebApi.Interfaces;

namespace TsuShopWebApi.Services;

public class CartService : ICartService
{
    private readonly IApplicationDbContext _context;
    private readonly IProductService _productService;

    public CartService(IApplicationDbContext context, IProductService productService)
    {
        _context = context;
        _productService = productService;
    }

    public async Task<ICollection<CartItem>?> GetAllCartItemsAsync(Guid cartId, Guid userId)
    {
        var cart = await GetCartAsync(cartId: cartId, userId: userId);

        if (cart is null)
            return null;

        return cart.CartItems;
    }

    public async Task<bool> AddSomeInCartAsync(Guid cartId, Guid productId, int quantity, Guid userId)
    {
        var cart = await GetCartAsync(cartId: cartId, userId: userId);

        if (cart is null)
            return false; // is not cart owner

        bool? availability = await _productService.IsAvailableAsync(productId, quantity);

        if (availability is false)
            return false; // product is not available or not found

        var cartItem = cart.CartItems.FirstOrDefault(cartItem => cartItem.ProductId == productId);

        if (cartItem != null)
        {
            cartItem.Quantity += quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var newCartItem = new CartItem
            {
                Id = Guid.NewGuid(),

                Quantity = quantity,
                ProductId = productId,
                CartId = cartId,

                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _context.CartItems.AddAsync(newCartItem);
        }

        if (await _context.SaveChangesAsync() > 0)
        {
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> RemoveSomeFromCartAsync(Guid cartId, Guid cartItemId, Guid userId,
        int quantity = Int32.MaxValue)
    {
        var cart = await GetCartAsync(cartId: cartId, userId: userId);
        if (cart is null)
            return false; // 

        var cartItem = await _context.CartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.CartId == cartId);

        if (cartItem == null)
            return false;


        if (cartItem.Quantity <= quantity)
        {
            _context.CartItems.Remove(cartItem);
        }
        else
        {
            cartItem.Quantity -= quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;
        }

        if (await _context.SaveChangesAsync() > 0)
        {
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> BuyAsync(Guid userId)
    {
        throw new NotImplementedException();
    }


    private async Task<Cart?> GetCartAsync(Guid cartId, Guid userId)
    {
        return await _context.Carts
            .Include(cart => cart.CartItems)
            .FirstOrDefaultAsync(cart => cart.Id == cartId && cart.UserId == userId);
    }
}