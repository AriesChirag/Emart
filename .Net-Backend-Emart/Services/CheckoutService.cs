using Emart_DotNet.Repositories;

namespace Emart_DotNet.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICartRepository _cartRepository;

        public CheckoutService(ICustomerRepository customerRepository, ICartRepository cartRepository)
        {
            _customerRepository = customerRepository;
            _cartRepository = cartRepository;
        }

        public async Task<object> SelectDeliveryOptionAsync(int userId, string deliveryType)
        {
            var customer = await _customerRepository.FindByUserIdAsync(userId);
            if (customer == null)
            {
                throw new Exception("Customer not found");
            }

            var cart = await _cartRepository.GetByCustomerAsync(userId);
            if (cart == null)
            {
                throw new Exception("Cart not found");
            }

            // Example logic mirroring Java
            if (!deliveryType.Equals("DELIVERY", StringComparison.OrdinalIgnoreCase) &&
                !deliveryType.Equals("PICKUP", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid delivery type");
            }

            // Logic to save delivery type to cart would go here if cart has the field
            // cart.DeliveryType = ...; 
            // await _cartRepository.SaveAsync(cart);

            return "Delivery option '" + deliveryType + "' selected successfully";
        }

        public async Task<object> PlaceOrderAsync(int userId)
        {
            var customer = await _customerRepository.FindByUserIdAsync(userId);
            if (customer == null)
            {
                throw new Exception("Customer not found");
            }

            var cart = await _cartRepository.GetByCustomerAsync(userId);
            if (cart == null)
            {
                throw new Exception("Cart not found");
            }

            if (cart.TotalAmount == 0) // primitives are non-null in C# struct logic unless nullable, assuming default 0
            {
                 throw new Exception("Cart is empty");
            }
             // Java uses CompareTo(BigDecimal.ZERO), C# decimal uses == 0

             // Future logic: Create Order, Move Items, Clear Cart

            return "Order placed successfully for userId: " + userId;
        }
    }
}
