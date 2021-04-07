using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System.ComponentModel;
using System.Linq;

namespace SportsStore.Controllers
{
    public class HomeController : Controller
    {
        private IStoreRepository repository;
        public int PageSize = 4;

        public HomeController(IStoreRepository repo)
        {
            repository = repo;
        }
        public IActionResult Index(string category, int productPage = 1)
        {
            int totalItems = category == null ? repository.Products.Count() : repository.Products.Where(p => p.Category == category).Count();

            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = totalItems
            };

            ProductsListViewModel productsListViewModel = new ProductsListViewModel
            {
                Products = repository.Products
                .Where(p => category == null || p.Category == category)
                    .OrderBy(p => p.ProductID)
                    .Skip((productPage - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = pagingInfo, 
                CurrentCategory = category
            };

            return View(productsListViewModel);
        }
    }
}
