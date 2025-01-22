using TsuShopWebApi.Entities;

namespace TsuShopWebApi.Interfaces;

public interface ICartService
{
    Task<ICollection<CartItem>?> GetAllCartItemsAsync(Guid userId);

    Task<bool> ChangeQuantityAsync(Guid productId, int quantity, Guid userId);
    Task<bool> AddSomeInCartAsync(Guid productId, int quantity, Guid userId);
    Task<bool> RemoveItemFromCartAsync(Guid cartItemId, Guid userId);

    Task<bool> BuyAsync(Guid userId);
}