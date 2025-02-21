namespace TsuShopWebApi.Models.ProductModels;

public class CreateProductRequestModel
{
    public string Name { get; set; }
    public IFormFile Image { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
}