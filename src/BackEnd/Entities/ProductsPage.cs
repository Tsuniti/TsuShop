namespace TsuShopWebApi.Entities;

public class ProductsPage
{
    public ICollection<Product> Products { get; set; }
    public int Pages { get; set; }
}