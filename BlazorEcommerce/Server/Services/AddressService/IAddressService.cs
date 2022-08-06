namespace BlazorEcommerce.Server.Services.AddressService
{
    public interface IAddressService
    {
        public Task<ServiceResponse<Address>> GetAddress();
        public Task<ServiceResponse<Address>> AddOrUpdateAddress(Address address);
    }
}
