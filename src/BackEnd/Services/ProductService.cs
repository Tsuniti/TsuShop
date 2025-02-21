using Microsoft.EntityFrameworkCore;
using TsuShopWebApi.Entities;
using TsuShopWebApi.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using TsuShopWebApi.Options;

namespace TsuShopWebApi.Services;

public class ProductService : IProductService
{
    
    private readonly IApplicationDbContext _context;
    private readonly IUserService _userService;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _blobContainerName;

    public ProductService(IApplicationDbContext context,
        IUserService userService,
        IOptions<AzureBlobStorageOptions> azureBlobServiceOptions)
    {
        _context = context;
        _userService = userService;
        _blobServiceClient = new BlobServiceClient(azureBlobServiceOptions.Value.ConnectionString);
        _blobContainerName = azureBlobServiceOptions.Value.ContainerName;
    }
    
    
    public async Task<ProductsPage> GetSomeAsync(int page = 1, int quantity = 21, int minPrice = 0, int maxPrice = Int32.MaxValue, string? sortBy = null, bool isAscending = true, string? category = null, string? name = null)
    {

        var productsQuery = _context.Products.AsNoTracking().AsQueryable().Where(product => product.Price <= maxPrice && product.Price >= minPrice);


        if (!string.IsNullOrEmpty(category))
        {
            productsQuery = productsQuery.Where(product => product.Category == category);
        }

        if (!string.IsNullOrEmpty(name))
        {
            productsQuery = productsQuery.Where(product => product.Name.Contains(name));
        }
        
        if (!string.IsNullOrEmpty(sortBy))
        {
            if (isAscending is false) // если не указано или false, то по убыванию, иначе по возрастанию
            {
                // Сортировка по убыванию
                productsQuery = productsQuery.OrderByDescending(product => EF.Property<object>(product, sortBy));

            }
            else
            {
                // Сортировка по возрастанию
                productsQuery = productsQuery.OrderBy(product => EF.Property<object>(product, sortBy));
            }
        }
        else
        {
            if (isAscending is false)
            {

                // По умолчанию сортировка по имени
                productsQuery = productsQuery.OrderBy(product => product.Name);
            }
            else
            {
                productsQuery = productsQuery.OrderByDescending(product => product.Name);

            }
        }
        
        var pages = quantity > 0 ? Math.Ceiling((double)productsQuery.Count() / quantity) : 0;


        // Ограничение по количеству
        var products = await productsQuery
                                                    .Skip(quantity * (page - 1))
                                                    .Take(quantity)
                                                    .ToListAsync();

        var productsPage = new ProductsPage
        {
            Products = products,
            Pages = (int)pages
        };

        return productsPage;
        
    }

    public async Task<ICollection<string>> GetCategories()
    {
        // Извлекаем уникальные категории из продуктов
        var categories = await _context.Products
            .Select(p => p.Category)
            .Distinct() // Получаем уникальные категории
            .ToListAsync();

        return categories;
    }

    public async Task<Product?> GetByIdAsync(Guid productId)
    {
        return await _context.Products
            .Include(product => product.Reviews)
            .ThenInclude(review => review.User)
            .FirstOrDefaultAsync(product => product.Id == productId);
    }

    public async Task<Product?> CreateAsync(string name, IFormFile file, string description, string category, double price, int quantity, Guid userId)
    {
        if (!await _userService.IsUserAdminAsync(userId)) 
            return null;

        
        var newProduct = new Product
        {

            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Category = category,
            Price = price,
            Rating = 0,
            Quantity = quantity,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
            
        };


        newProduct.ImageUrl = await UploadImageAsync(file, newProduct.Id);


        await _context.Products.AddAsync(newProduct);
        
        return await _context.SaveChangesAsync() > 0 ? newProduct : null;
    }
    
    public async Task<Product?> UpdateAsync(Guid productId, string name, IFormFile? file, string description, string category, double price, int quantity,
        Guid userId)
    {
        var product = await GetByIdAsync(productId);
        if (product is null || !await _userService.IsUserAdminAsync(userId))
            return null;

        product.Name = name;
        product.Description = description;
        product.Category = category;
        product.Price = price;
        product.Quantity = quantity;

        product.UpdatedAt = DateTime.UtcNow;

        if (file is not null)
        {
            if (!await DeleteImageAsync(product.ImageUrl))
            {
                return null;
            }

            product.ImageUrl = await UploadImageAsync(file, product.Id);
        }

        return await _context.SaveChangesAsync() > 0 ? product : null;
    }

    public async Task<bool> DeleteAsync(Guid productId, Guid userId)
    {
        if (!await _userService.IsUserAdminAsync(userId)) 
            return false;
        
        var product = await GetByIdAsync(productId);

        if (product is null)
            return false;

        if (!await DeleteImageAsync(product.ImageUrl))
        {
            return false;
        }

        _context.Products.Remove(product);
        return await _context.SaveChangesAsync() > 0;

    }

    public async Task<bool> RemoveSomeFromStockAsync(Guid productId, int quantity)
    {
        var product = await GetByIdAsync(productId);

        if (product is null)
            return false;

        product.Quantity -= quantity;
        product.UpdatedAt = DateTime.UtcNow;
        
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddSomeToStockAsync(Guid productId, int quantity, Guid userId)
    {
        if (!await _userService.IsUserAdminAsync(userId)) 
            return false;
        
        var product = await GetByIdAsync(productId);

        if (product is null)
            return false;

        product.Quantity += quantity;
        product.UpdatedAt = DateTime.UtcNow;
        
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<double> GetMaxPrice() => await _context.Products.MaxAsync(p => p.Price);
    

    public async Task<bool> ReCountRatingAsync(Product product)
    {
        Console.WriteLine("recounting "+ product.Reviews.Count);
        if (product.Reviews.Count == 0)
        {
            product.Rating = 0; // если нет отзывов, рейтинг равен 0 или как-то иначе
            return await _context.SaveChangesAsync() > 0;
        }
        
        double sum = 0;

        foreach (var r in product.Reviews)
        {
            sum += r.Rating;
        }
        product.Rating = sum / product.Reviews.Count;
        Console.WriteLine(sum / product.Reviews.Count);
        Console.WriteLine(product.Rating);
        
        
        return await _context.SaveChangesAsync() > 0;
    }

    private async Task<string> UploadImageAsync(IFormFile file, Guid productId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        string fileName = $"{productId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var blobClient = containerClient.GetBlobClient(fileName);

        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
        }

        return blobClient.Uri.ToString();
    }

    private async Task<bool> DeleteImageAsync(string imageUrl)
    {
        try
        {
            var uri = new Uri(imageUrl);
            var fileName = Path.GetFileName(uri.LocalPath);

            // Получаем клиент для контейнера Blob
            var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            // Удаляем изображение
            await blobClient.DeleteIfExistsAsync();

            return true;
        }
        catch (Exception ex)
        {
            // Логируем ошибку
            Console.WriteLine($"Error deleting blob: {ex.Message}");
            return false; // Ошибка при удалении
        }
    }
}