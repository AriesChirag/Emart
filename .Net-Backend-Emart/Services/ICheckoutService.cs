namespace Emart_DotNet.Services
{
    public interface ICheckoutService
    {
        Task<object> SelectDeliveryOptionAsync(int userId, string deliveryType);
        Task<object> PlaceOrderAsync(int userId);
    }
}
