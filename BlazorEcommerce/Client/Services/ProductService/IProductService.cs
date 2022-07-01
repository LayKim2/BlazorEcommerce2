namespace BlazorEcommerce.Client.Services.ProductService;

public interface IProductService
{
    event Action ProductsChanged;
    List<Product> Products { get; set; }

    Task GetProducts();
    //Task GetProducts(string? category = null);

    Task<ServiceResponse<Product>> GetProduct(int productId);

}
