using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SportsStore.Tests
{
    public class HomeControllerTests
    {
        private IStoreRepository CreateIStoreRepository()
        {
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product {ProductID = 1, Name= "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name= "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name= "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name= "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name= "P5", Category = "Cat3"},
            }).AsQueryable<Product>);

            return mock.Object;
        }


        [Fact]
        public void Can_Use_Repository()
        {
            // arrange
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns(
                (new Product[] 
                {
                    new Product { ProductID = 1, Name = "P1" },
                    new Product { ProductID = 2, Name = "P2" }
                }).AsQueryable<Product>);

            HomeController homeController = new HomeController(mock.Object);

            // act
            //IEnumerable<Product> result = (homeController.Index() as ViewResult).ViewData.Model as IEnumerable<Product>;
            ProductsListViewModel result = (homeController.Index(null) as ViewResult).ViewData.Model as ProductsListViewModel;

            // assert
            Product[] productArray = result.Products.ToArray();
            Assert.True(productArray.Length == 2);
            Assert.Equal(1, productArray[0].ProductID);
            Assert.Equal("P1", productArray[0].Name);
            Assert.Equal(2, productArray[1].ProductID);
            Assert.Equal("P2", productArray[1].Name);
        }

        [Fact]
        public void Can_Paginate()
        {
            // arrange
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[] 
            {
                new Product {ProductID = 1, Name= "P1"},
                new Product {ProductID = 2, Name= "P2"},
                new Product {ProductID = 3, Name= "P3"},
                new Product {ProductID = 4, Name= "P4"},
                new Product {ProductID = 5, Name= "P5"},
            }).AsQueryable<Product>);

            HomeController controller = new HomeController(mock.Object);
            controller.PageSize = 3;

            // act
            //IEnumerable<Product> resultPage1 = (controller.Index() as ViewResult).ViewData.Model as IEnumerable<Product>;
            //IEnumerable<Product> resultPage2 = (controller.Index(2) as ViewResult).ViewData.Model as IEnumerable<Product>;
            ProductsListViewModel resultPage1 = (controller.Index(null) as ViewResult).ViewData.Model as ProductsListViewModel;
            ProductsListViewModel resultPage2 = (controller.Index(null, 2) as ViewResult).ViewData.Model as ProductsListViewModel;

            // assert
            //Product[] prodArrayPage1 = resultPage1.ToArray();
            Product[] prodArrayPage1 = resultPage1.Products.ToArray();
            Assert.True(prodArrayPage1.Length == 3);
            Assert.Equal("P1", prodArrayPage1[0].Name);
            Assert.Equal("P2", prodArrayPage1[1].Name);
            Assert.Equal("P3", prodArrayPage1[2].Name);

            //Product[] prodArrayPage2 = resultPage2.ToArray();
            Product[] prodArrayPage2 = resultPage2.Products.ToArray();
            Assert.True(prodArrayPage2.Length == 2);
            Assert.Equal("P4", prodArrayPage2[0].Name);
            Assert.Equal("P5", prodArrayPage2[1].Name);

        }

        [Fact]
        public void Can_Filter_Products()
        {
            // arrange
            IStoreRepository repo = CreateIStoreRepository();
            HomeController controller = new HomeController(repo);
            controller.PageSize = 3;

            // act
            Product[] result = ((controller.Index("Cat2", 1) as ViewResult).ViewData.Model as ProductsListViewModel).Products.ToArray();

            // assert
            Assert.Equal(2, result.Length);
            Assert.True(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.True(result[1].Name == "P4" && result[0].Category == "Cat2");
        }

        [Fact]
        public void Generate_Category_Specific_Product_Count()
        {
            // arrange
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product { ProductID = 1, Name = "P1", Category = "Cat1" },
                new Product { ProductID = 2, Name = "P2", Category = "Cat2" },
                new Product { ProductID = 3, Name = "P3", Category = "Cat1" },
                new Product { ProductID = 4, Name = "P4", Category = "Cat2" },
                new Product { ProductID = 5, Name = "P5", Category = "Cat3" }
            }).AsQueryable<Product>);
            HomeController target = new HomeController(mock.Object);
            target.PageSize = 3;
            Func<ViewResult, ProductsListViewModel> GetModel = result => result?.ViewData?.Model as ProductsListViewModel;

            // act
            int? result1 = GetModel(target.Index("Cat1") as ViewResult)?.PagingInfo.TotalItems;
            int? result2 = GetModel(target.Index("Cat2") as ViewResult)?.PagingInfo.TotalItems;
            int? result3 = GetModel(target.Index("Cat3") as ViewResult)?.PagingInfo.TotalItems;
            int? resultAll = GetModel(target.Index(null) as ViewResult)?.PagingInfo.TotalItems;

            // assert
            Assert.Equal(2, result1);
            Assert.Equal(2, result2);
            Assert.Equal(1, result3);
            Assert.Equal(5, resultAll);
        }

    }
}
