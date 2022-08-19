using System.Net.Http.Json;

// client 의 service는 js/jquery의 ajax 느낌을 봐야겠다.

namespace BlazorEcommerce.Client.Services.ProductService;

public class ProductService : IProductService
{
    private readonly HttpClient _http;

    public ProductService(HttpClient http)
    {
        _http = http;
    }

    public List<Product> Products { get; set; } = new List<Product>();
    public string Message { get; set; } = "Loading Products...";
    public int CurrentPage { get; set; } = 1;
    public int PageCount { get; set; } = 0;
    public string LastSearchText { get; set; } = string.Empty;
    public List<Product> AdminProducts { get; set; } = new List<Product>();

    public event Action ProductsChanged;

    public async Task<Product> CreateProduct(Product product)
    {
        var result = await _http.PostAsJsonAsync("api/product", product);
        var newProduct = (await result.Content.ReadFromJsonAsync<ServiceResponse<Product>>()).Data;

        return newProduct;
    }

    public async Task DeleteProduct(Product product)
    {
        var result = await _http.DeleteAsync($"api/product/{product.Id}");
    }

    public async Task GetAdminProducts()
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product/admin");

        AdminProducts = result.Data;
        CurrentPage = 1;
        PageCount = 0;

        if(AdminProducts.Count == 0)
            Message = "No products found.";
    }

    public async Task<ServiceResponse<Product>> GetProduct(int productId)
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<Product>>($"api/product/{productId}");

        return result;
    }

    public async Task<ServiceResponse<Product>> GetProductByCategory(int categoryUrl)
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<Product>>($"api/product/category/{categoryUrl}");

        return result;
    }

    public async Task GetProducts(string? categoryUrl = null)
    {
        #region no pagination
        var result = categoryUrl == null ?
            await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product/featured") :
            await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/category/{categoryUrl}"); 
        #endregion

        if (result != null && result.Data != null)
            Products = result.Data;

        CurrentPage = 1;
        PageCount = 0;

        if(Products.Count == 0)
        {
            Message = "No products found";
        }

        ProductsChanged.Invoke();
    }

    public async Task<List<string>> GetProductSearchSuggestions(string searchText)
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<string>>>($"api/product/searchsuggestions/{searchText}");

        return result.Data;
    }

    public async Task SearchProducts(string searchText, int page)
    {
        LastSearchText = searchText;

        var result = await _http.GetFromJsonAsync<ServiceResponse<ProductSearhResult>>($"api/product/search/{searchText}/{page}");

        #region no pagination
        // Products = result.Data; 
        #endregion

        if (result != null && result.Data != null)
        {
            Products = result.Data.Products;
            CurrentPage = result.Data.CurrentPage;
            PageCount = result.Data.Pages;
        }
            
        if (Products.Count == 0)
            Message = "No Product found.";

        ProductsChanged?.Invoke();
    }

    public async Task<Product> UpdateProduct(Product product)
    {
        var result = await _http.PutAsJsonAsync("api/product", product);

        return (await result.Content.ReadFromJsonAsync<ServiceResponse<Product>>()).Data;
    }



    #region noCategoryDisplayProducts
    //public async Task GetProducts()
    //{
    //    var result = await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product");

    //    if (result != null && result.Data != null)
    //        Products = result.Data;
    //}
    #endregion

}
