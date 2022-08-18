namespace BlazorEcommerce.Server.Services.ProductTypeService;

public class ProductTypeService : IProductTypeService
{
    private readonly DataContext _context;

    public ProductTypeService(DataContext context)
    {
        _context = context;
    }
    public async Task<ServiceResponse<List<ProductType>>> GetProductTypes()
    {
        var productTypes = await _context.ProductTypes.ToListAsync();

        return new ServiceResponse<List<ProductType>> { Data = productTypes };
    }
}
