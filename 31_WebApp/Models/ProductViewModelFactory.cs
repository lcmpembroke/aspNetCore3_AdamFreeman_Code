using System.Collections.Generic;
using System.Linq;

namespace WebApp.Models
{
    public static class ProductViewModelFactory
    {
        public static ProductViewModel Details(Product p)
        {
            return new ProductViewModel {
                Product = p,
                Action = "Details",
                Theme = "info",
                ReadOnly = true,
                ShowAction = false,
                Categories = p == null ? Enumerable.Empty<Category>() : new List<Category> { p.Category },
                Suppliers = p == null ? Enumerable.Empty<Supplier>() : new List<Supplier> { p.Supplier },
            };
        }


        public static ProductViewModel Create(Product p, IEnumerable<Category> categories, IEnumerable<Supplier> suppliers)
        {
            return new ProductViewModel { 
                Product = p, Categories = categories, Suppliers = suppliers
            };
        }

        public static ProductViewModel Edit(Product p, IEnumerable<Category> categories, IEnumerable<Supplier> suppliers)
        {
            return new ProductViewModel
            {
                Product = p,
                Action = "Edit",
                ReadOnly = false,
                ShowAction = true,
                Theme = "warning",
                Categories = categories,
                Suppliers = suppliers
            };
        }

        public static ProductViewModel Delete(Product p)
        {
            return new ProductViewModel
            {
                Product = p,
                Action = "Delete",
                ReadOnly = true,
                Theme = "danger",
                Categories = p == null ? Enumerable.Empty<Category>() : new List<Category> { p.Category },
                Suppliers = p == null ? Enumerable.Empty<Supplier>() : new List<Supplier> { p.Supplier },
            };
        }


    }
}