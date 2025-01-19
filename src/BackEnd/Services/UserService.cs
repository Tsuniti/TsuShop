using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TsuShopWebApi.Entities;
using TsuShopWebApi.Interfaces;

namespace TsuShopWebApi.Services;

public class UserService : IUserService
{
    private readonly IApplicationDbContext _context;

    public UserService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateUserAsync(string username, string password)
    {
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = Hash(password),
            IsAdmin = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var newCart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = newUser.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Users.AddAsync(newUser);
        await _context.Carts.AddAsync(newCart);
        await _context.SaveChangesAsync();

        return newUser.Id;
    }

    public async Task<bool> UsernameExistsAsync(string username)
        => await _context.Users.AnyAsync(user => user.Username.Equals(username));
    


    public async Task<Guid?> GetUserIdByCredentialsAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Username.Equals(username));

        if (user is null)
            return null;

        string hash = Hash(password);

        if (!user.PasswordHash.Equals(hash))
            return null;

        return user.Id;
    }

    // if user not exist return also false
    public async Task<bool> IsUserAdminAsync(Guid id)
        =>  await _context.Users.FirstOrDefaultAsync(user => user.Id == id) is {IsAdmin: true}; 
    

    // Захешировать строку str по алгоритму SHA256
    private string Hash(string str)
    {
        var hashValue = SHA256.HashData(Encoding.UTF8.GetBytes(str));
        return BitConverter.ToString(hashValue);
    }
}