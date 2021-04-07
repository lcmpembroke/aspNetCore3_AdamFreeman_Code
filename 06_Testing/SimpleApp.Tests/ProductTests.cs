using SimpleApp.Models;
using Xunit;

namespace SimpleApp.Tests
{
    public class ProductTests
    {
        [Fact]
        public void CanChangeProductName()
        {
            // arrange
            var p = new Product { Name = "Test", Price = 100M };

            // act
            p.Name = "New name";

            // assert
            Assert.Equal("New name", p.Name);
        }

        [Fact]
        public void CanChangeProductPrice()
        {
            // arrange
            var p = new Product { Name = "Test", Price = 100M };

            // act
            p.Price = 200M;

            // assert
            Assert.Equal(200M, p.Price);
        }

    }
}
