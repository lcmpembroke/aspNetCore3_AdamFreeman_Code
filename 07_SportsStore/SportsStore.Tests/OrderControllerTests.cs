using Moq;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Controllers;
using SportsStore.Models;
using Xunit;
using System.Linq;

namespace SportsStore.Tests
{
    public class OrderControllerTests
    {
        [Fact]
        public void Cannot_Checkout_Empty_Cart()
        {
            // arrange ----------------
            // create mock repository, empty cart, new order and an instance of the orderController
            Mock<IOrderRepository> mockOrderRepo = new Mock<IOrderRepository>();
            Cart cart = new Cart();
            Order order = new Order();
            OrderController target = new OrderController(mockOrderRepo.Object, cart);


            // act ----------------
            ViewResult result = target.Checkout(order) as ViewResult;


            // assert ----------------
            // check order has not been stored by checking SaveOrder of the mock IOrderRepository implementation has NEVER been called
            mockOrderRepo.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);

            // check method is returning the default view
            Assert.True(string.IsNullOrEmpty(result.ViewName));

            // check an invalud modeal is passed to the view
            Assert.False(result.ViewData.ModelState.IsValid);
        }



        [Fact]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            // arrange
            // create mock repository, create cart with one item, create instance of orderController, add error to the model
            Mock<IOrderRepository> mockOrderRepo = new Mock<IOrderRepository>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            OrderController target = new OrderController(mockOrderRepo.Object, cart);
            target.ModelState.AddModelError("errorKey", "errorMessage");

            // act - try to check out
            ViewResult result = target.Checkout(new Order()) as ViewResult;

            // assert
            // check order has not been stored
            mockOrderRepo.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);

            // check method returns default view
            Assert.True(string.IsNullOrEmpty(result.ViewName));

            // check invalid model passed to view
            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public void Can_Checkout_And_Submit_Order()
        {
            // arrange
            // create mock repository, create cart with one item, create instance of orderController
            Mock<IOrderRepository> mockOrderRepo = new Mock<IOrderRepository>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            OrderController target = new OrderController(mockOrderRepo.Object, cart);

            // act
            // try to check out - Checkout() fills the new order object from cart, then saves order to order repository, then redirects to "Completed" Razor page
            RedirectToPageResult result = target.Checkout(new Order()) as RedirectToPageResult;

            // assert
            // check order has been stored
            mockOrderRepo.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Once);

            Assert.Equal("/Completed", result.PageName);
        }

    }
}
