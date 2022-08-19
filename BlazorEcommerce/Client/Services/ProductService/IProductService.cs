namespace BlazorEcommerce.Client.Services.ProductService;

public interface IProductService
{
    event Action ProductsChanged;
    List<Product> Products { get; set; }
    List<Product> AdminProducts { get; set; }

    string Message { get; set; }

    #region you need this when you move page.

    public int CurrentPage { get; set; }
    public int PageCount { get; set; }
    public string LastSearchText { get; set; }

    #endregion

    //Task GetProducts();
    Task GetProducts(string? category = null);
    Task GetAdminProducts();

    Task<ServiceResponse<Product>> GetProduct(int productId);

    //Task<ServiceResponse<Product>> SearchProducts(string searchText);

    Task SearchProducts(string searchText, int page);
    Task<List<string>> GetProductSearchSuggestions(string searchText);

    Task<Product> CreateProduct(Product product);
    Task<Product> UpdateProduct(Product product);
    Task DeleteProduct(Product product);

}
