﻿using System.Net.Http.Json;

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

    public event Action ProductsChanged;

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

    //public async Task GetProducts(string? categoryUrl = null)
    //{
    //    var result = categoryUrl == null ?
    //        await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product") :
    //        await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/category/{categoryUrl}");

    //    if (result != null && result.Data != null)
    //        Products = result.Data;


    //    ProductsChanged.Invoke();
    //}

    #region noCategoryDisplayProducts
    public async Task GetProducts()
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product");

        if (result != null && result.Data != null)
            Products = result.Data;
    }
    #endregion

}