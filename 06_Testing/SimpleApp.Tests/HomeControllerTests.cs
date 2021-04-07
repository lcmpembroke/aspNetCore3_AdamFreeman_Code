using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SimpleApp.Controllers;
using SimpleApp.Models;
using Xunit;
using Moq;

namespace SimpleApp.Tests
{
    public class HomeControllerTests
    {

        [Fact]
        public void IndexActionModelIsComplete()
        {
            // arrange
            Product[] testData = new Product[]
            {
                new Product { Name="P1", Price=75.10M},
                new Product { Name="P2", Price=120M},
                new Product { Name="P3", Price=110M}
            };
            var mock = new Mock<IDataSource>();
            mock.SetupGet(m => m.Products).Returns(testData);   // SetupGet implements the getter for a property. Lambda expression specifies property to be implemented
            var controller = new HomeController { dataSource = mock.Object };

            // act
            var model = (controller.Index() as ViewResult)?.ViewData.Model as IEnumerable<Product>;

            // assert
            Assert.Equal(testData,
                        model,
                        Comparer.Get<Product>((p1, p2) => p1.Name == p2.Name && p1.Price == p2.Price)
                        );
            // checks the Products property was called only once
            mock.VerifyGet(m => m.Products, Times.Once);
        }




        // page 119 - not needed once Moq is used
        class FakeDataSource : IDataSource
        {
            public FakeDataSource(Product[] data) => Products = data;
            public IEnumerable<Product> Products { get; set; }
        }

        [Fact]
        public void IndexActionModelIsComplete_usingFakeDataSource_p119()
        {
            // arrange
            Product[] testData = new Product[]
            {
                new Product { Name="P1", Price=75.10M},
                new Product { Name="P2", Price=120M},
                new Product { Name="P3", Price=110M}
            };
            IDataSource data = new FakeDataSource(testData);
            var controller = new HomeController { dataSource = data };

            // act
            var model = (controller.Index() as ViewResult)?.ViewData.Model as IEnumerable<Product>;

            // assert
            Assert.Equal(data.Products,
                        model,
                        Comparer.Get<Product>((p1, p2) => p1.Name == p2.Name && p1.Price == p2.Price)
                        );
        }


        [Fact]
        public void IndexActionModelIsComplete_beforeIsolatingController_page117()
        {
            // arrange
            var controller = new HomeController();
            Product[] products = new Product[]
            {
                new Product { Name="Kayak", Price=275M},
                new Product { Name="Lifejacket", Price=48.95M},
            };

            // act
            var model = (controller.Index() as ViewResult)?.ViewData.Model as IEnumerable<Product>;

            // assert
            Assert.Equal(products,
                        model, 
                        Comparer.Get<Product>((p1,p2) => p1.Name == p2.Name && p1.Price == p2.Price)
                        );
        }
    }
}
