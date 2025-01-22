using TsuShopWebApi.Entities;

namespace TsuShopWebApi.Interfaces;

public interface ICartService
{
    Task<ICollection<CartItem>?> GetAllCartItemsAsync(Guid userId);
    Task<bool> AddSomeInCartAsync(Guid productId, int quantity, Guid userId);
    Task<bool> RemoveSomeFromCartAsync(Guid cartItemId, Guid userId, int quantity);

    Task<bool> BuyAsync(Guid userId);
}