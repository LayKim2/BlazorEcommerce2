namespace BlazorEcommerce.Server.Services.ProductService
{
    public interface IProductService
    {
        Task<ServiceResponse<List<Product>>> GetProductAsync();

        Task<ServiceResponse<Product>> GetProductAsync(int productId);
    }
}
