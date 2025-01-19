using TsuShopWebApi.Entities;

namespace TsuShopWebApi.Interfaces;

public interface ICartService
{
    Task<ICollection<CartItem>?> GetAllCartItemsAsync(Guid cartId, Guid userId);
    Task<bool> AddSomeInCartAsync(Guid cartId, Guid productId, int quantity, Guid userId);
    Task<bool> RemoveSomeFromCartAsync(Guid cartId, Guid cartItemId, Guid userId, int quantity);

    Task<bool> BuyAsync(Guid userId);
}