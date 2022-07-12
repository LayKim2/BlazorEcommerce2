namespace BlazorEcommerce.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;

        public ProductService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<Product>>> GetFeaturedProducts()
        {
            var response = new ServiceResponse<List<Product>>()
            {
                Data = await _context.Products
                                .Where(p => p.Featured)
                                .Include(p => p.Variants)
                                .ToListAsync()
            };

            return response;
        }


        #region list
        public async Task<ServiceResponse<List<Product>>> GetProductAsync()
        {
            var products = await _context.Products.ToListAsync();
            var response = new ServiceResponse<List<Product>>()
            {
                Data = await _context.Products.Include(p => p.Variants).ToListAsync()
            };

            return response;
        }
        #endregion

        #region detail
        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            var response = new ServiceResponse<Product>();

            var product = await _context.Products
                .Include(p => p.Variants)
                .ThenInclude(v => v.ProductType)
                .FirstOrDefaultAsync(p => p.Id == productId);
            //.FindAsync(productId);

            if (product == null)
            {
                response.Success = false;
                response.Message = "Sorry, but this product does not exist.";
            }
            else
            {
                response.Data = product;
            }

            return response;
        }
        #endregion

        public async Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl)
        {
            #region no pagination
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products
                        .Where(p => p.Category.Url.ToLower().Equals(categoryUrl.ToLower()))
                        .Include(p => p.Variants)
                        .ToListAsync()
            }; 
            #endregion

            return response;
        }

        public async Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText)
        {
            var products = await FindProductsBySearchText(searchText);

            List<string> result = new List<string>();

            foreach(var product in products)
            {
                if (product.Ttile.Contains(searchText, StringComparison.OrdinalIgnoreCase)){
                    result.Add(product.Ttile);
                }

                if(product.Description != null)
                {
                    var punctuation = product.Description.Where(char.IsPunctuation)
                            .Distinct().ToArray();

                    var words = product.Description.Split()
                            .Select(s => s.Trim(punctuation));

                    foreach(var word in words)
                    {
                        if (word.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                            && !result.Contains(word))
                        {
                            result.Add(word);
                        }

                    }
                }
            }

            return new ServiceResponse<List<string>> { Data = result };
        }

        public async Task<ServiceResponse<ProductSearhResult>> SearchProducts(string searchText, int page)
        {
            var pageResults = 2f;
            var pageCount = Math.Ceiling((await FindProductsBySearchText(searchText)).Count / pageResults);
            var products = await _context.Products
                                .Where(p => p.Ttile.ToLower().Contains(searchText.ToLower()) || p.Description.ToLower().Contains(searchText.ToLower()))
                                //.Where(p => searchText.Contains(p.Ttile) || searchText.Contains(p.Description))
                                .Include(p => p.Variants)
                                .Skip((page - 1) * (int)pageResults)
                                .Take((int)pageResults)
                                .ToListAsync();

            var response = new ServiceResponse<ProductSearhResult>()
            {
                Data = new ProductSearhResult()
                {
                    Products = products,
                    CurrentPage = page,
                    Pages = (int)pageCount
                }
            };

            #region not use pagination
            //var response = new ServiceResponse<ProductSearhResult>
            //{
            //    Data = await FindProductsBySearchText(searchText)
            //};
            #endregion 


            return response;
        }

        private async Task<List<Product>> FindProductsBySearchText(string searchText)
        {
            return await _context.Products
                                .Where(p => p.Ttile.ToLower().Contains(searchText.ToLower()) || p.Description.ToLower().Contains(searchText.ToLower()))
                                //.Where(p => searchText.Contains(p.Ttile) || searchText.Contains(p.Description))
                                .Include(p => p.Variants)
                                .ToListAsync();
        }
    }
}
