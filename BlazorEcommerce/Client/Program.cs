using BlazorEcommerce.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Blazored.LocalStorage
builder.Services.AddBlazoredLocalStorage();

// mudblazor
builder.Services.AddMudServices();

// radzen blazor
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

// DI(Dependency Injection)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();

// Authentication
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();


await builder.Build().RunAsync();
