using Stripe;
using Stripe.Checkout;

namespace BlazorEcommerce.Server.Services.PaymentService;

public class PaymentService : IPaymentService
{
    private readonly ICartService _cartService;
    private readonly IAuthService _authService;
    private readonly IOrderService _orderService;

    const string secret = "whsec_98edb1dcdbdb3893936555512631780a30ff609750c9bf5469187d19223c56c3";

    public PaymentService(ICartService cartService,
                        IAuthService authService,
                        IOrderService orderService)
    {
        StripeConfiguration.ApiKey = "sk_test_51LSv2kKmdSbhcvh7nVg7IdGNMWjbMt8tUeut6VTetuQmPpWlt9mVUs0O1BOQzbb5jlP1KRWW9ewkEpuZRKWOyjkE00GQ58Hg1O";

        _cartService = cartService;
        _authService = authService;
        _orderService = orderService;
    }

    public async Task<Session> CreateCheckoutSession()
    {
        var products = (await _cartService.GetDbCartProducts()).Data;
        var lineItems = new List<SessionLineItemOptions>();

        products.ForEach(product => lineItems.Add(new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmountDecimal = product.Price * 100,
                Currency = "usd",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = product.Title,
                    Images = new List<string> { product.ImageUrl }
                }
            },
            Quantity = product.Quantity,
        }));
         
        var options = new SessionCreateOptions
        {
            CustomerEmail = _authService.GetUserEmail(),
            ShippingAddressCollection = new SessionShippingAddressCollectionOptions
            {
                AllowedCountries = new List<string> { "US" }
            },
            PaymentMethodTypes = new List<string>
            {
                "card",
            },
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = "https://localhost:7061/order-success",
            CancelUrl = "https://localhost:7061/cart"
        };

        var service = new SessionService();
        Session session = service.Create(options);

        return session;

    }

    public async Task<ServiceResponse<bool>> FulfillOrder(HttpRequest request)
    {
        var json = await new StreamReader(request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    request.Headers["Stripe-Signature"],
                    secret
                );

            if (stripeEvent.Type == Events.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;
                var user = await _authService.GetUserByEmail(session.CustomerEmail);
                await _orderService.PlaceOrder(user.Id);
            }

            return new ServiceResponse<bool> { Data = true };
        }
        catch (StripeException e)
        {
            return new ServiceResponse<bool> { Data = false, Success = false, Message = e.Message };
        }

    }
}
