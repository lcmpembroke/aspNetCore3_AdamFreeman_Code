
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;

namespace LanguageFeatures.Models
{
    public static class MyExtensionMethods
    {
        // BOOK p91 second argument is a function that accepts a product object, and returns a bool value
        public static IEnumerable<Product> Filter(this IEnumerable<Product> productEnumerable, Func<Product, bool> selector)
        {
            foreach (Product product in productEnumerable)
            {
                if (selector(product))
                {
                    yield return product;
                }
            }
        }

        public static IEnumerable<Product> FilterByName(this IEnumerable<Product> productEnumerable, char firstLetter)
        {
            foreach (Product product in productEnumerable)
            {
                if (product?.Name?[0] == firstLetter)
                {
                    yield return product;
                }
            }
        }

        // BOOK p88 - create filtering extension method...uses "yield" keyword
        public static IEnumerable<Product> FilterByPrice(this IEnumerable<Product> productEnumerable, decimal minPrice)
        {
            foreach (Product p in productEnumerable)
            {
                // if product is null, set pice to zero
                if ((p?.Price ?? 0) >= minPrice)
                {
                    yield return p;
                }
            }
        }

        // BOOK p86 - entension method applied to interface (more flexible as don't need a specific implementation)
        // eg can have arrays of Products, or a ShoppingCart
        public static decimal TotalPrices(this IEnumerable<Product> products)
        {
            decimal total = 0;
            foreach (Product p in products)
            {
                total += p?.Price ?? 0;
            }
            return total;
        }

        // BOOK p85 pretending the ShoppingCart is third party code so cannot be modified - hence using extension method for needed functionality
        public static decimal TotalPrices_before_Interface_Implemented(this ShoppingCart cartParam)
        {
            decimal total = 0;
            foreach (Product p in cartParam.Products)
            {
                total += p?.Price ?? 0;
            }
            return total;
        }
    }
}
