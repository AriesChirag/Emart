using Emart_DotNet.DTOs;
using Emart_DotNet.Models;
using Emart_DotNet.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emart_DotNet.Services
{
    

    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepo;
        private readonly ICartItemRepository _cartItemRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IProductRepository _productRepo;

        private static readonly decimal PLATFORM_FEE = 23.00m;
        private static readonly decimal EPOINT_TO_RUPEE = 1.00m;

        public CartService(
            ICartRepository cartRepo,
            ICartItemRepository cartItemRepo,
            ICustomerRepository customerRepo,
            IProductRepository productRepo)
        {
            _cartRepo = cartRepo;
            _cartItemRepo = cartItemRepo;
            _customerRepo = customerRepo;
            _productRepo = productRepo;
        }

        public async Task<CartItem> AddToCartAsync(int userId, int productId, int quantity, string purchaseType, int epointsUsed)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero");

            var customer = await _customerRepo.FindByUserIdAsync(userId)
                           ?? throw new Exception("Customer not found");

            int availablePoints = customer.Epoints ?? 0;

            var cart = await _cartRepo.GetByCustomerAsync(userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = customer.UserId,
                    TotalMrp = 0,
                    CouponDiscount = 0,
                    PlatformFee = 0,
                    UsedEpoints = 0,
                    EpointDiscount = 0,
                    EarnedEpoints = 0,
                    FinalPayableAmount = 0,
                    TotalAmount = 0
                };
                cart = await _cartRepo.SaveAsync(cart);
            }

            var product = await _productRepo.FindByIdAsync(productId)
                          ?? throw new Exception("Product not found");

            var item = await _cartItemRepo.FindByCartAndProductAsync(cart.CartId, product.ProductId);
            if (item == null)
            {
                item = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = product.ProductId,
                    EpointsUsed = 0,
                    Quantity = 0
                };
            }

            int finalQuantity = (item.CartItemId == 0) ? quantity : (item.Quantity ?? 0) + quantity;
            string finalPurchaseType = purchaseType ?? "NORMAL";
            int finalEpointsUsed = 0;

            decimal effectivePrice = product.GetEffectivePrice();

            // Validate Eligibility for e-Points
            if (finalPurchaseType == "FULL_EP" || finalPurchaseType == "PARTIAL_EP")
            {
                // Check for e-MART Card
                if (customer.CardHolder == null)
                {
                     throw new Exception("E-Points redemption is only available for e-MART Card holders.");
                }
            }

            if (finalPurchaseType == "FULL_EP")
            {
                finalEpointsUsed = (int)(effectivePrice * finalQuantity);
                if (finalEpointsUsed > availablePoints)
                {
                    throw new Exception($"Insufficient e-points. Available: {availablePoints}");
                }
            }
            else if (finalPurchaseType == "PARTIAL_EP")
            {
                decimal partialRatio = 0.37m;
                decimal totalItemPrice = effectivePrice * finalQuantity;
                int requiredPoints = (int)Math.Ceiling(totalItemPrice * partialRatio);

                if (requiredPoints > availablePoints)
                {
                    throw new Exception($"Insufficient e-points for 37% redemption. Required: {requiredPoints}, Available: {availablePoints}");
                }
                finalEpointsUsed = requiredPoints;
            }

            item.Quantity = finalQuantity;
            item.PurchaseType = finalPurchaseType;
            item.EpointsUsed = finalEpointsUsed;
            
            decimal subtotal = effectivePrice * finalQuantity;
            item.Subtotal = subtotal;
            item.TotalPrice = subtotal;

            item = await _cartItemRepo.SaveAsync(item);
            
            // Reload Cart to get fresh items
            cart = await _cartRepo.GetByCustomerAsync(userId); // Ensure we have latest items
            await RecalculateCartPricing(cart);

            return item;
        }

        public async Task<CartItem> UpdateQuantityAsync(int cartItemId, int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero");

            var item = await _cartItemRepo.FindByIdAsync(cartItemId)
                       ?? throw new Exception("Cart item not found");

            item.Quantity = quantity;
            decimal effectivePrice = item.Product.GetEffectivePrice();

            // Normalize epointsUsed if switching back to NORMAL (though backend logic keeps type usually)
            if (string.IsNullOrEmpty(item.PurchaseType) || item.PurchaseType == "NORMAL")
            {
                item.EpointsUsed = 0;
            }

            if (item.PurchaseType == "FULL_EP")
            {
                int required = (int)(effectivePrice * quantity);
                
                // Validation for update
                int available = item.Cart.User.Epoints ?? 0;
                if (required > available) {
                     throw new Exception($"Insufficient e-points for updated quantity. Required: {required}, Available: {available}");
                }

                item.EpointsUsed = required;
            }
            
            if (item.PurchaseType == "PARTIAL_EP")
            {
                 decimal partialRatio = 0.37m;
                 decimal totalItemPrice = effectivePrice * quantity;
                 int requiredPoints = (int)Math.Ceiling(totalItemPrice * partialRatio);
                 
                 int available = item.Cart.User.Epoints ?? 0;
                 if (requiredPoints > available) {
                      throw new Exception($"Insufficient e-points for updated quantity. Required: {requiredPoints}, Available: {available}");
                 }
                 
                 item.EpointsUsed = requiredPoints;
            }

            decimal subtotal = effectivePrice * quantity;
            item.Subtotal = subtotal;
            item.TotalPrice = subtotal;

            item = await _cartItemRepo.SaveAsync(item);
            
            // Reload cart to ensure all items are present for recalc
            var cart = await _cartRepo.GetByCustomerAsync(item.Cart.UserId ?? 0);
            await RecalculateCartPricing(cart);

            return item;
        }

        public async Task RemoveFromCartAsync(int cartItemId)
        {
            var item = await _cartItemRepo.FindByIdAsync(cartItemId)
                       ?? throw new Exception("Cart item not found");
            
            var cart = item.Cart;
            await _cartItemRepo.DeleteAsync(item);
            
            // Recalc
             // Need to fetch cart again to get updated items list or just use existing cart if repo deletion reflects?
             // Safest to fetch by ID
             var updatedCart = await _cartRepo.GetByCustomerAsync(cart.UserId ?? 0);
             await RecalculateCartPricing(updatedCart);
        }

        public async Task ClearCartByUserAsync(int userId)
        {
            var cart = await _cartRepo.GetByCustomerAsync(userId)
                 ?? throw new Exception("Cart not found");
            
            await _cartItemRepo.DeleteByCartIdAsync(cart.CartId);

            cart.TotalMrp = 0;
            cart.CouponDiscount = 0;
            cart.PlatformFee = 0;
            cart.UsedEpoints = 0;
            cart.EpointDiscount = 0;
            cart.EarnedEpoints = 0;
            cart.FinalPayableAmount = 0;
            cart.TotalAmount = 0;
            
            await _cartRepo.SaveAsync(cart);
        }

        public async Task<CartResponseDTO> GetCartSummaryAsync(int userId)
        {
             // Ensure cart exists
            var customer = await _customerRepo.FindByUserIdAsync(userId)
                           ?? throw new Exception("Customer not found");

            var cart = await _cartRepo.GetByCustomerAsync(userId);
            if (cart == null)
            {
                 // Create empty cart
                cart = new Cart
                {
                    UserId = customer.UserId,
                    TotalMrp = 0,
                    CouponDiscount = 0,
                    PlatformFee = 0,
                    UsedEpoints = 0,
                    EpointDiscount = 0,
                    EarnedEpoints = 0,
                    FinalPayableAmount = 0,
                    TotalAmount = 0
                };
                cart = await _cartRepo.SaveAsync(cart);
            }
            
            await RecalculateCartPricing(cart);
            return ConvertToDTO(cart, customer);
        }

        private async Task RecalculateCartPricing(Cart cart)
        {
            // Fetch Items if not loaded
            var items = await _cartItemRepo.FindByCartAsync(cart.CartId);

            decimal totalMrp = 0;
            int totalEpointsUsed = 0;

            foreach (var item in items)
            {
                decimal itemPrice = item.Product.GetEffectivePrice();
                totalMrp += itemPrice * (item.Quantity ?? 0);
                totalEpointsUsed += (item.EpointsUsed ?? 0);
            }

            cart.TotalMrp = totalMrp;
            cart.PlatformFee = items.Any() ? PLATFORM_FEE : 0;
            
            decimal epointDiscount = totalEpointsUsed * EPOINT_TO_RUPEE;
            if (epointDiscount > totalMrp) epointDiscount = totalMrp;
            
            cart.UsedEpoints = totalEpointsUsed;
            cart.EpointDiscount = epointDiscount;
            
            decimal couponDiscount = cart.CouponDiscount ?? 0;
            
            decimal finalAmount = totalMrp - epointDiscount - couponDiscount + (cart.PlatformFee ?? 0);
            if (finalAmount < 0) finalAmount = 0;
            
            // GST 10%
            decimal gstAmount = finalAmount * 0.10m;
            decimal finalPayableWithGst = finalAmount + gstAmount;
            
            cart.FinalPayableAmount = finalPayableWithGst;
            cart.TotalAmount = finalPayableWithGst;
            
            decimal cashPaid = totalMrp - epointDiscount - couponDiscount;
            if (cashPaid < 0) cashPaid = 0;
            
            cart.EarnedEpoints = (int)(cashPaid * 0.10m);

            await _cartRepo.SaveAsync(cart);
        }

        private CartResponseDTO ConvertToDTO(Cart cart, Customer customer)
        {
            CartResponseDTO dto = new CartResponseDTO();
            
             // Calculate Real Total MRP (Sum of Normal Prices)
            decimal realTotalMrp = 0;
            var items = cart.CartItems; // Assuming loaded

            List<CartItemDTO> itemDTOs = new List<CartItemDTO>();
            foreach (var item in items)
            {
                var d = new CartItemDTO
                {
                    CartItemId = item.CartItemId,
                    ProductId = item.ProductId ?? 0,
                    ProductName = item.Product?.ProductName,
                    Quantity = item.Quantity ?? 0,
                    Price = item.Product?.NormalPrice ?? 0,
                    DiscountedPrice = item.Product?.GetEffectivePrice() ?? 0,
                    PurchaseType = item.PurchaseType,
                    EpointsUsed = item.EpointsUsed ?? 0,
                    ImageUrl = item.Product?.ProductImageUrl
                };
                itemDTOs.Add(d);
                
                realTotalMrp += (item.Product?.NormalPrice ?? 0) * (item.Quantity ?? 0);
            }
            
            dto.Items = itemDTOs;
            dto.TotalMrp = realTotalMrp;
            dto.OfferDiscount = realTotalMrp - (cart.TotalMrp ?? 0);
            
            dto.EpointDiscount = cart.EpointDiscount ?? 0;
            dto.CouponDiscount = cart.CouponDiscount ?? 0;
            dto.PlatformFee = cart.PlatformFee ?? 0;
            
            dto.FinalPayableAmount = cart.FinalPayableAmount ?? 0;
            dto.TotalAmount = cart.FinalPayableAmount ?? 0;
            
            decimal preGst = (cart.FinalPayableAmount ?? 0) / 1.1m;
            dto.GstAmount = (cart.FinalPayableAmount ?? 0) - preGst; // Approximation reverse calc to match Java
            
            dto.UsedEpoints = cart.UsedEpoints ?? 0;
            dto.EarnedEpoints = cart.EarnedEpoints ?? 0;
            dto.AvailableEpoints = customer.Epoints ?? 0;

            return dto;
        }
    }
}
