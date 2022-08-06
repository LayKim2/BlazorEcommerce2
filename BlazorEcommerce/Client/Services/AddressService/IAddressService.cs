namespace BlazorEcommerce.Client.Services.AddressService;

public interface IAddressService
{
    public Task<Address> GetAddress();
    public Task<Address> AddOrUpdateAddress(Address address);
}
