﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using SportsStore.Components;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SportsStore.Tests
{
    public class NavigationMenuViewComponentTests
    {
        [Fact]
        public void Can_Select_Categories()
        {
            // arrange
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product {ProductID = 1, Name= "P1", Category = "Apples"},
                new Product {ProductID = 2, Name= "P2", Category = "Apples"},
                new Product {ProductID = 3, Name= "P3", Category = "Plums"},
                new Product {ProductID = 4, Name= "P4", Category = "Oranges"},
                new Product {ProductID = 5, Name= "P5", Category = "Bananas"},
            }).AsQueryable<Product>());

            NavigationMenuViewComponent target = new NavigationMenuViewComponent(mock.Object);

            // act
            string[] results = ((IEnumerable<string>)(target.Invoke() as ViewViewComponentResult).ViewData.Model).ToArray();

            // assert
            Assert.True(Enumerable.SequenceEqual(new string[] { "Apples","Bananas","Oranges","Plums" }, results));
        }

        [Fact]
        public void Indicates_Selected_Category()
        {
            // arrange
            string categoryToSelect = "Apples";
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product {ProductID = 1, Name= "P1", Category = "Apples"},
                new Product {ProductID = 4, Name= "P2", Category = "Plums"}
            }).AsQueryable<Product>());

            NavigationMenuViewComponent target = new NavigationMenuViewComponent(mock.Object);
            target.ViewComponentContext = new ViewComponentContext 
                                        { ViewContext = 
                                            new ViewContext 
                                                { RouteData = new Microsoft.AspNetCore.Routing.RouteData() } 
                                        };
            target.RouteData.Values["category"] = categoryToSelect;

            // act
            string result = (string)(target.Invoke() as ViewViewComponentResult).ViewData["SelectedCategory"];

            // assert
            Assert.Equal(categoryToSelect,result);
        }
    }
}