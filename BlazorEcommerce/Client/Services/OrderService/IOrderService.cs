namespace BlazorEcommerce.Client.Services.OrderService;

public interface IOrderService
{
    Task PlaceOrder();
    Task<List<OrderOverviewResponse>> GetOrder();
    Task<OrderDetailsResponse> GetOrderDetails(int orderId);
}
