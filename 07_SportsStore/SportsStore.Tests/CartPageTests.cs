using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using SportsStore.Models;
using SportsStore.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Xunit;

namespace SportsStore.Tests
{
    public class CartPageTests
    {
        [Fact]
        public void Can_Load_Cart()
        {
            // arrange
            // create mock repository of products
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Mock<IStoreRepository> mockRepo = new Mock<IStoreRepository>();
            mockRepo.Setup(m => m.Products).Returns((new Product[] { p1, p2 }).AsQueryable<Product>);

            // create a cart
            Cart testCart = new Cart();
            testCart.AddItem(p1, 2);
            testCart.AddItem(p2, 1);

            // create a mock session to use to provide the session for the mock page HttpContext
            //
            //  Useful notes:
            //  ISession Interface -Stores user data(as byte array) while the user browses a web application.Session state uses a store maintained by the application to persist data across requests from a client.The session data is backed by a cache and considered ephemeral data.
            //  (Namespace: Microsoft.AspNetCore.Http)
            Mock<ISession> mockSession = new Mock<ISession>();
            byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(testCart));
            mockSession.Setup(c => c.TryGetValue(It.IsAny<string>(), out data));
            
            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);

            // act
            // NB: Cart is now provided as a constructor argument (not accessed through the Context objects
            //  as had to do before "CartService was implemented

            CartModel cartPageModel = new CartModel(mockRepo.Object, testCart);
            cartPageModel.OnGet("myUrl");

            // assert
            Assert.Equal(2, cartPageModel.Cart.Lines.Count());
            Assert.Equal("myUrl", cartPageModel.ReturnUrl);
        }

        [Fact]
        public void Can_Update_Cart()
        {
            // Shared/ProductSummary.cshtml posts a form to /Cart Razor page sending through ProductId and returnUrl when "Add to cart" is pressed for product
            // i.e. testing CartModel.OnPost(long productId, string returnUrl)

            // arrange
            // create mock repository of products
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Mock<IStoreRepository> mockRepo = new Mock<IStoreRepository>();
            mockRepo.Setup(m => m.Products).Returns((new Product[] { p1, p2 }).AsQueryable<Product>);

            // create a cart
            Cart testCart = new Cart();

            // create the Model for the CartPage
            CartModel cartModel = new CartModel(mockRepo.Object, testCart);

            // act
            cartModel.OnPost(p1.ProductID, "myUrl");

            // assert
            Assert.Single(testCart.Lines);
            Assert.Equal("P1", testCart.Lines.First().Product.Name);
            Assert.Equal(1, testCart.Lines.First().Quantity);
        }

    }
}
