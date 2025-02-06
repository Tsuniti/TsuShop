namespace TsuShopWebApi.Models.ProductModels;

public class RemoveSomeFromStockRequestModel
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}