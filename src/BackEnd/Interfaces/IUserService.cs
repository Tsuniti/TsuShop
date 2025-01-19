namespace TsuShopWebApi.Interfaces;

public interface IUserService
{
    // Создать нового пользователя
    // Возвращаем айди пользователя
    Task<Guid> CreateUserAsync(string username, string password);

    // Занят ли логин
    Task<bool> UsernameExistsAsync(string username);

    // Получить айди пользователя по логину и паролю
    Task<Guid?> GetUserIdByCredentialsAsync(string username, string password);
    Task<bool> IsUserAdminAsync(Guid id);

}