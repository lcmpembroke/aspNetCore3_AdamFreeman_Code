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
            // USEFUL NOTES for the following;
            //--------------------------------
            // PageContext Class = The context associated with the current request for a Razor page.
            //      (Namespace: Microsoft.AspNetCore.Mvc.RazorPages)
            //
            // ActionContext Class = Context object for execution of action which has been selected as part of an HTTP request.
            //      (Namespace: Microsoft.AspNetCore.Mvc)
            //      Properties:
            //      ActionDescriptor - Gets or sets the ActionDescriptor for the selected action.
            //      HttpContext - Gets or sets the HttpContext for the current request.
            //      ModelState - Gets the ModelStateDictionary.
            //      RouteData - Gets or sets the RouteData for the current request.

            CartModel cartPageModel = new CartModel(mockRepo.Object)
            { 
                PageContext = new PageContext(new ActionContext 
                                            { 
                                                HttpContext = mockHttpContext.Object, 
                                                RouteData = new RouteData(), 
                                                ActionDescriptor = new PageActionDescriptor() 
                                            } )
            };
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
            testCart.AddItem(p1, 2);
            testCart.AddItem(p2, 1);

            //
            //create an HttpSession as neededin the OnPost() method
            Mock<ISession> mockSession = new Mock<ISession>();
            mockSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Callback<string, byte[]>((key,val) => 
                {
                    testCart = JsonSerializer.Deserialize<Cart>(Encoding.UTF8.GetString(val));
                });

            // create an HttpContext and set it's session to the mockSession just created about
            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.SetupGet(c => c.Session).Returns(mockSession.Object);

            // create the Model for the CartPage and associate the mockHttpContext (that has the mock Session ) with it
            CartModel cartModel = new CartModel(mockRepo.Object)
            {
                PageContext = new PageContext(new ActionContext 
                                                { 
                                                    HttpContext = mockHttpContext.Object, 
                                                    RouteData = new RouteData(), 
                                                    ActionDescriptor = new PageActionDescriptor() 
                                                })
            };


            // act
            cartModel.OnPost(p1.ProductID, "myUrl");

            // assert
            Assert.Single(testCart.Lines);
            Assert.Equal("P1", testCart.Lines.First().Product.Name);
            Assert.Equal(1, testCart.Lines.First().Quantity);
        }

    }
}
