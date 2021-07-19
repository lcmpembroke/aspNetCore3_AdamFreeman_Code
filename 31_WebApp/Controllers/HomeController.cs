using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Filters;
using WebApp.Models;

namespace WebApp.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {
        private DataContext context;

        private IEnumerable<Category> _categories => context.Categories;
        private IEnumerable<Supplier> _suppliers => context.Suppliers;

        public HomeController(DataContext data)
        {
            context = data;
        }

        public IActionResult Index()
        {
            return View(context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                );
        }

        public async Task<IActionResult> Details(long id)
        {
            Product p = await context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            return View("ProductEditor", ProductViewModelFactory.Details(p));
        }


        public IActionResult Create()
        {
            return View("ProductEditor", new ProductViewModel { Product = new Product(), Categories = _categories, Suppliers = _suppliers });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Product product)
        {
            if (ModelState.IsValid)
            {
                // reset these properties to ensure
                //  - primary key for productId is allocated by database server
                //  - and to prevent EFCore dealing with related data here
                product.ProductId = default;
                product.Category = default;
                product.Supplier = default;
                context.Products.Add(product);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("ProductEditor", ProductViewModelFactory.Create(product, _categories, _suppliers));
        }

        public async Task<IActionResult> Edit(long id)
        {
            Product p = await context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            return View("ProductEditor", ProductViewModelFactory.Edit(p, _categories, _suppliers));
        }


        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] Product product)
        {
            //var errors = ModelState
            //    .Where(x => x.Value.Errors.Count > 0)
            //    .Select(x => new { x.Key, x.Value.Errors })
            //    .ToArray();

            if (ModelState.IsValid)
            {
                product.Category = default;
                product.Supplier = default;
                context.Products.Update(product);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("ProductEditor", ProductViewModelFactory.Edit(product, _categories, _suppliers));
        }


        public async Task<IActionResult> Delete(long id)
        {
            //Product p = await context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            Product p = await context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            return View("ProductEditor", ProductViewModelFactory.Delete(p));
        }

        [HttpPost]  // Product object created by model binding from form data
        public async Task<IActionResult> Delete(Product product)
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}