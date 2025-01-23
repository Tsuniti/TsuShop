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

    public async Task<ICollection<CartItem>?> GetAllCartItemsAsync(Guid userId)
    {
        var cart = await _context.Carts
            .Where(cart => cart.User.Id == userId)
            .Include(cart => cart.CartItems)
            .ThenInclude(cartItems => cartItems.Product)
            .FirstOrDefaultAsync();

        return cart.CartItems;
    }


    public async Task<bool> ChangeQuantityAsync(Guid productId, int quantity, Guid userId)
    {
        var cart = await _context.Carts
            .Where(cart => cart.User.Id == userId)
            .Include(cart => cart.CartItems)
            .ThenInclude(cartItems => cartItems.Product)
            .FirstOrDefaultAsync();

        if (cart is null)
            return false;

        var cartItem = cart.CartItems.FirstOrDefault(cartItem => cartItem.ProductId == productId);
        if (cartItem is null)
            return false;

        var product = cartItem.Product;
        
        if (product.Quantity < quantity)
        {
            return false;
        }

        cartItem.Quantity = quantity;
        cartItem.UpdatedAt = DateTime.UtcNow;

        if (cartItem.Quantity <= 0)
        {
            _context.CartItems.Remove(cartItem);
        }

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddSomeInCartAsync(Guid productId, int quantity, Guid userId)
    {
        var cart = await _context.Carts
            .Where(cart => cart.User.Id == userId)
            .Include(cart => cart.CartItems)
            .FirstOrDefaultAsync();

        if (cart is null)
            return false;


        if (cart.CartItems.Any(cartItem => cartItem.ProductId == productId))
        {
            if (await ChangeQuantityAsync(productId, quantity, userId))
            {
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        var newCartItem = new CartItem
        {
            Id = Guid.NewGuid(),

            Quantity = quantity,
            ProductId = productId,
            CartId = cart.Id,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _context.CartItems.AddAsync(newCartItem);


        if (await _context.SaveChangesAsync() > 0)
        {
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
    

    public async Task<bool> RemoveItemFromCartAsync(Guid cartItemId, Guid userId)
    {
        var cart = await _context.Carts
            .Where(cart => cart.User.Id == userId)
            .Include(cart => cart.CartItems)
            .FirstOrDefaultAsync();
        
        if (cart is null)
            return false;

        var cartItem = cart.CartItems.FirstOrDefault(cartItem => cartItem.Id == cartItemId);

        if (cartItem == null)
            return false;


        _context.CartItems.Remove(cartItem);


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
    
}