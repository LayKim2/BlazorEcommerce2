namespace BlazorEcommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;
        private readonly AuthenticationStateProvider _authStateProvider;

        public event Action OnChange;

        public CartService(ILocalStorageService localStorage, HttpClient http
            , AuthenticationStateProvider authStateProvider)
        {
            _localStorage = localStorage;
            _http = http;
            _authStateProvider = authStateProvider;
        }

        public async Task AddToCart(CartItem cartItem)
        {
            if (await IsAuthenticated())
            {
                await _http.PostAsJsonAsync("api/cart/add",cartItem);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                if (cart == null)
                {
                    cart = new List<CartItem>();
                }

                var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId && x.ProductTypeId == cartItem.ProductTypeId);

                if (sameItem == null)
                {
                    cart.Add(cartItem);
                }
                else
                {
                    sameItem.Quantity += cartItem.Quantity;
                }

                await _localStorage.SetItemAsync("cart", cart);
            }

            await GetCartItemsCount();
            //OnChange.Invoke();

        }

        //public async Task AddToCart<T>(string listName, T item)
        //{
        //    List<T> list = await _localStorage.GetItemAsync<List<T>>(listName);

        //    if (list is null)
        //        list = new();

        //    list.Add(item);

        //    await _localStorage.SetItemAsync(listName, list);

        //    OnChange.Invoke();
        //}


        //public async Task<List<CartItem>> GetCartItems()
        //{
        //    await GetCartItemsCount();

        //    var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        //    if (cart == null)
        //        cart = new List<CartItem>();

        //    return cart;
        //}

        public async Task GetCartItemsCount()
        {
            if(await IsAuthenticated())
            {
                var result = await _http.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
                var count = result.Data;

                await _localStorage.SetItemAsync<int>("cartItemsCount", count);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                await _localStorage.SetItemAsync<int>("cartItemsCount", cart != null ? cart.Count : 0);
            }

            OnChange.Invoke();
        }

        public async Task<List<CartProductResponse>> GetCartProducts()
        {
            //var cartItems = GetCartItems();

            // login이 되어있으면, db에서 가져오기
            if(await IsAuthenticated())
            {
                var response = await _http.GetFromJsonAsync<ServiceResponse<List<CartProductResponse>>> ("api/cart");
                return response.Data;
            }
            else // login이 되어있지 않으면, local storage에서 가져오기
            {
                var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");

                if(cartItems == null)
                {
                    return new List<CartProductResponse>();
                }

                var response = await _http.PostAsJsonAsync("api/cart/products", cartItems);
                var cartProducts = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();

                return cartProducts.Data;
            }
            
        }

        public async Task RemoveProductFromCart(int productId, int productTypeId)
        {
            if( await IsAuthenticated())
            {
                await _http.DeleteAsync($"api/cart/{productId}/{productTypeId}");
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");

                if (cart == null)
                {
                    return;
                }

                var cartItem = cart.Find(x => x.ProductId == productId && x.ProductTypeId == productTypeId);

                if (cartItem != null)
                {
                    cart.Remove(cartItem);
                    await _localStorage.SetItemAsync("cart", cart);
                    //OnChange.Invoke();
                }
            }

            //await GetCartItemsCount();
        }

        // local storage에 있는 cart item을 db에 저장
        public async Task StoreCartItems(bool emptyLocalCart = false)
        {
            var localCart = await _localStorage.GetItemAsync<List<CartItem>>("cart");

            if (localCart == null)
            {
                return;
            }

            await _http.PostAsJsonAsync("api/cart", localCart);

            if (emptyLocalCart)
            {
                await _localStorage.RemoveItemAsync("cart");
            }
        }

        public async Task UpdateQuantity(CartProductResponse product)
        {
            if( await IsAuthenticated())
            {
                var request = new CartItem
                {
                    ProductId = product.ProductId,
                    ProductTypeId = product.ProductTypeId,
                    Quantity = product.Quantity,
                };

                await _http.PutAsJsonAsync("api/cart/update-quantity", request);
            } 
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");

                if (cart == null)
                {
                    return;
                }

                var cartItem = cart.Find(x => x.ProductId == product.ProductId && x.ProductTypeId == product.ProductTypeId);

                if (cartItem != null)
                {
                    cartItem.Quantity = product.Quantity;
                    await _localStorage.SetItemAsync("cart", cart);
                    //OnChange.Invoke();
                }
            }

            
        }

        private async Task<bool> IsAuthenticated()
        {
            return (await _authStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }

    }
}
