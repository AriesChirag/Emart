using Emart_DotNet.Controllers;
using Emart_DotNet.Services;
using Emart_DotNet.DTOs;
using Emart_DotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Emart_DotNet.Tests.Controllers
{
    /// <summary>
    /// Unit tests for CartController
    /// Tests all cart operations: add, update, remove, get summary, clear, and view
    /// </summary>
    public class CartControllerTests
    {
        private readonly Mock<ICartService> _mockCartService;
        private readonly CartController _controller;

        public CartControllerTests()
        {
            _mockCartService = new Mock<ICartService>();
            _controller = new CartController(_mockCartService.Object);
        }

        #region AddToCart Tests

        [Fact]
        public async Task AddToCart_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var expectedItem = new CartItem 
            { 
                CartItemId = 1, 
                ProductId = 10, 
                Quantity = 2,
                TotalPrice = 200.00m 
            };
            
            _mockCartService
                .Setup(s => s.AddToCartAsync(1, 10, 2, "NORMAL", 0))
                .ReturnsAsync(expectedItem);

            // Act
            var result = await _controller.AddToCart(1, 10, 2, "NORMAL", 0);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedItem = Assert.IsType<CartItem>(okResult.Value);
            Assert.Equal(10, returnedItem.ProductId);
            Assert.Equal(2, returnedItem.Quantity);
        }

        [Fact]
        public async Task AddToCart_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            _mockCartService
                .Setup(s => s.AddToCartAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Product not found"));

            // Act
            var result = await _controller.AddToCart(1, 999, 1, "NORMAL", 0);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Product not found", badRequestResult.Value);
        }

        [Fact]
        public async Task AddToCart_WithEPoints_PassesCorrectParameters()
        {
            // Arrange
            var expectedItem = new CartItem { CartItemId = 1, ProductId = 5 };
            
            _mockCartService
                .Setup(s => s.AddToCartAsync(1, 5, 3, "ECARD", 50))
                .ReturnsAsync(expectedItem);

            // Act
            var result = await _controller.AddToCart(1, 5, 3, "ECARD", 50);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockCartService.Verify(s => s.AddToCartAsync(1, 5, 3, "ECARD", 50), Times.Once);
        }

        #endregion

        #region UpdateQuantity Tests

        [Fact]
        public async Task UpdateQuantity_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var expectedItem = new CartItem { CartItemId = 1, Quantity = 5 };
            
            _mockCartService
                .Setup(s => s.UpdateQuantityAsync(1, 5))
                .ReturnsAsync(expectedItem);

            // Act
            var result = await _controller.UpdateQuantity(1, 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedItem = Assert.IsType<CartItem>(okResult.Value);
            Assert.Equal(5, returnedItem.Quantity);
        }

        [Fact]
        public async Task UpdateQuantity_CartItemNotFound_ReturnsBadRequest()
        {
            // Arrange
            _mockCartService
                .Setup(s => s.UpdateQuantityAsync(999, 1))
                .ThrowsAsync(new Exception("Cart item not found"));

            // Act
            var result = await _controller.UpdateQuantity(999, 1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cart item not found", badRequestResult.Value);
        }

        #endregion

        #region RemoveFromCart Tests

        [Fact]
        public async Task RemoveFromCart_ValidCartItemId_ReturnsOkResult()
        {
            // Arrange
            _mockCartService
                .Setup(s => s.RemoveFromCartAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RemoveFromCart(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Item removed successfully", okResult.Value);
        }

        [Fact]
        public async Task RemoveFromCart_CartItemNotFound_ReturnsBadRequest()
        {
            // Arrange
            _mockCartService
                .Setup(s => s.RemoveFromCartAsync(999))
                .ThrowsAsync(new Exception("Cart item not found"));

            // Act
            var result = await _controller.RemoveFromCart(999);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cart item not found", badRequestResult.Value);
        }

        #endregion

        #region GetCartSummary Tests

        [Fact]
        public async Task GetCartSummary_ValidUserId_ReturnsOkWithSummary()
        {
            // Arrange
            var expectedSummary = new CartResponseDTO
            {
                TotalAmount = 500.00m,
                FinalPayableAmount = 450.00m,
                Items = new List<CartItemDTO>
                {
                    new CartItemDTO { CartItemId = 1, ProductId = 10, Quantity = 2 },
                    new CartItemDTO { CartItemId = 2, ProductId = 20, Quantity = 1 }
                }
            };
            
            _mockCartService
                .Setup(s => s.GetCartSummaryAsync(1))
                .ReturnsAsync(expectedSummary);

            // Act
            var result = await _controller.GetCartSummary(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedSummary = Assert.IsType<CartResponseDTO>(okResult.Value);
            Assert.Equal(500.00m, returnedSummary.TotalAmount);
            Assert.Equal(450.00m, returnedSummary.FinalPayableAmount);
        }

        [Fact]
        public async Task GetCartSummary_UserNotFound_ReturnsBadRequest()
        {
            // Arrange
            _mockCartService
                .Setup(s => s.GetCartSummaryAsync(999))
                .ThrowsAsync(new Exception("User not found"));

            // Act
            var result = await _controller.GetCartSummary(999);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User not found", badRequestResult.Value);
        }

        #endregion

        #region ClearCart Tests

        [Fact]
        public async Task ClearCart_ValidUserId_ReturnsOkResult()
        {
            // Arrange
            _mockCartService
                .Setup(s => s.ClearCartByUserAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ClearCart(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Cart cleared", okResult.Value);
        }

        [Fact]
        public async Task ClearCart_UserNotFound_ReturnsBadRequest()
        {
            // Arrange
            _mockCartService
                .Setup(s => s.ClearCartByUserAsync(999))
                .ThrowsAsync(new Exception("User not found"));

            // Act
            var result = await _controller.ClearCart(999);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User not found", badRequestResult.Value);
        }

        #endregion

        #region ViewCart Tests

        [Fact]
        public async Task ViewCart_ValidUserId_ReturnsCartSummary()
        {
            // Arrange
            var expectedSummary = new CartResponseDTO 
            { 
                TotalAmount = 200.00m 
            };
            
            _mockCartService
                .Setup(s => s.GetCartSummaryAsync(1))
                .ReturnsAsync(expectedSummary);

            // Act
            var result = await _controller.ViewCart(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedSummary = Assert.IsType<CartResponseDTO>(okResult.Value);
            Assert.Equal(200.00m, returnedSummary.TotalAmount);
        }

        #endregion
    }
}
