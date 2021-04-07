using LanguageFeatures.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageFeatures.Controllers
{
    public class HomeController : Controller
    {

        // BOOK p105 using 'nameof' to get names
        public ViewResult Index()
        {
            var products = new[]
            {
                new { Name = "Kayak", Price = 275M },
                new { Name = "Lifejacket", Price = 48.95M },
                new { Name = "Soccer ball", Price = 19.50M },
                new { Name = "Corner flag", Price = 34.95M }
            };
            return View(products.Select(p => $"{nameof(p.Name)}: {p.Name}, {nameof(p.Price)}: {p.Price}"));
        }



        // BOOK p103 - using an asynchronous Enumerable
        public async Task<ViewResult> Index_p103()
        {
            List<string> output = new List<string>();
            await foreach ( long? len in MyAsyncMethods.GetPageLengths(output,"apress.com","microsoft.com","amazon.co.uk"))
            {
                output.Add($"Page length: {len}");
            }
            return View(output);
        }

        public async Task<ViewResult> Index_8()
        {
            long? length = await MyAsyncMethods.GetPageLength();
            return View(new string[] { $"Length: {length}" });
        }

        // BOOK p98 update HomeController Index action to use interface for shopping cart
        public ViewResult Index_7()
        {
            IProductSelection cart = new ShoppingCart(
                    new Product { Name = "Kayak", Price = 275M },
                    new Product { Name = "Lifejacket", Price = 48.95M },
                    new Product { Name = "Soccer ball", Price = 19.50M },
                    new Product { Name = "Corner flag", Price = 34.95M }
            );

            //return View(cart.Products.Select(p => p.Name));   - commented out to use the default implementation of the names property (not defined in ShoppingCart, but in the interface IProductSlection)
            // BOOK p98 - adding a new feature to the interface - default implementations in interfaces feature added in C# 8.0 
            return View(cart.Names);
        }

        // BOOK p96 anonymous types and type inference (var)
        public ViewResult Index_6()
        {
            // each object in array is anonymously typed (type def'n will be created by compiler)
            // all objects on products array will have same compiler defined type as all define the same properties
            var products = new[] 
            {
                new { Name = "Kayak", Price = 275M },
                new { Name = "Lifejacket", Price = 48.95M },
                new { Name = "Soccer ball", Price = 19.50M },
                new { Name = "Corner flag", Price = 34.95M }
            };
            return View(products.Select(p => p.Name));
        }


        // BOOK p93 a Lambda action method
        public ViewResult Index_5()
        {
            return View(Product.GetProducts().Select(p => p?.Name));
        }


        // BOOK: page 86 - calling extension method of ShoppingCart  (extension method is defined in MyExtensionMethods class)
        //      and p88 adding filtering extension method
        // commented out as later updates to code breaks this code...
        //public ViewResult Index_4()
        //{
        //    ShoppingCart cart = new ShoppingCart { Products = Product.GetProducts() };

        //    Product[] productArray =
        //    {
        //        new Product {Name = "Kayak", Price = 275M},
        //        new Product {Name = "Lifejacket", Price = 48.95M},
        //        new Product {Name = "Soccer ball", Price = 19.50M},
        //        new Product {Name = "Corner flag", Price = 34.95M},
        //    };


        //    decimal cartTotal = cart.TotalPrices();
        //    // replacing use of particular filter functions with generic one that takes a lamba expression, to make filtering more flexible
        //    //decimal arrayPriceFilteredTotal = productArray.FilterByPrice(20).TotalPrices(); // only products with price higher than 20 selected
        //    //decimal arrayNameFilteredTotal = productArray.FilterByName('S').TotalPrices();  // only Soccerball price counted

        //    // use lambda expressions to filter, so only need to define one generic filter function extension method
        //    decimal arrayPriceFilteredTotal = productArray.Filter(product => ((product?.Price ?? 0) >= 20)).TotalPrices();
        //    decimal arrayNameFilteredTotal = productArray.Filter(product => (product?.Name?[0] == 'S')).TotalPrices();



        //    return View("Index", new string[] 
        //    { 
        //        $"Cart total: {cartTotal:C2}",
        //        $"Array price filtered by price total: {arrayPriceFilteredTotal:C2}",
        //        $"Array price filtered by name total: {arrayNameFilteredTotal:C2}"
        //    });
        //}

        // BOOK: page 83 - pattern matching in switch statements, and use of case...when... demo'd too
        public ViewResult Index_3()
        {
            object[] data = new object[] { 275M, 29.95M, "apple", "orange", 100, 10 };
            decimal total = 0;
            for (int i = 0; i < data.Length; i++)
            {
                switch (data[i])
                {
                    case decimal decimalValue:
                        total += decimalValue;
                        break;
                    case int intValue when intValue > 50:
                        total += intValue;
                        break;
                }
            }
            return View("Index", new string[] { $"Total: {total:C2}"});
        }


        // BOOK: page 82 - using collection initializer syntax
        public ViewResult Index_2()
        {
            //    Latest C# approach to initializing dictionary - more natural and consistent with the way values are retrieved/modified
            Dictionary<string, Product> products = new Dictionary<string, Product>
            {
                ["Kayak"] = new Product { Name = "Kayak", Price = 275M },
                ["Lifejacket"] = new Product { Name = "Lifejacket", Price = 48.95M }
            };
            return View("Index", products.Keys);
        }

        // BOOK: page 82 - initialize using curly brackets
        public ViewResult Index_1()
        {
            // Traditional C# approach to initializing dictionary - relies too much on the curly brackets
            Dictionary<string, Product> products = new Dictionary<string, Product>
            {
                { "Kayak", new Product { Name = "Kayak", Price = 275M } },
                { "Lifejacket", new Product { Name = "Lifejacket", Price = 48.95M } },
            };
            return View("Index", products.Keys);
        }

        // BOOK: page 73 - renamed Index_0 so I don't need to uncomment when updating
        public ViewResult Index_0()
        {
            List<string> results = new List<string>();

            foreach (Product p in Product.GetProducts())
            {
                string name = p?.Name ?? "<no name>";
                decimal? price = p?.Price ?? 0;
                string relatedname = p?.Related?.Name ?? "<none>";
                results.Add(string.Format("name: {0}, price: {1}, related: {2}", name, price, relatedname));
                results.Add($"name: {name}, price: {price:c2}, related: {relatedname}");
            }
            return View(results);
        }
    }
}
