namespace BlazorEcommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;

        public event Action OnChange;

        public CartService(ILocalStorageService localStorage, HttpClient http)
        {
            _localStorage = localStorage;
            _http = http;
        }
        public async Task AddToCart(CartItem cartItem)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if(cart == null)
            {
                cart = new List<CartItem>();
            }

            var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId && x.ProductTypeId == cartItem.ProductTypeId);

            if(sameItem == null)
            {
                cart.Add(cartItem);
            } else
            {
                sameItem.Quantity += cartItem.Quantity;
            }

            await _localStorage.SetItemAsync("cart", cart);

            OnChange.Invoke();
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


        public async Task<List<CartItem>> GetCartItems()
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null)
                cart = new List<CartItem>();

            return cart;
        }

        public async Task<List<CartProductResponse>> GetCartProducts()
        {
            //var cartItems = GetCartItems();

            var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");

            var response = await _http.PostAsJsonAsync("api/Cart/products", cartItems);

            var cartProducts = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();

            return cartProducts.Data;
        }

        public async Task RemoveProductFromCart(int productId, int productTypeId)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            
            if(cart == null)
            {
                return;
            }

            var cartItem = cart.Find(x => x.ProductId == productId && x.ProductTypeId == productTypeId);

            if(cartItem != null)
            {
                cart.Remove(cartItem);
                await _localStorage.SetItemAsync("cart", cart);
                OnChange.Invoke();
            }
                
        }

        public async Task UpdateQuantity(CartProductResponse product)
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
}
