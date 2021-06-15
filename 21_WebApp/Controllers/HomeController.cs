using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private DataContext context;

        public HomeController(DataContext dataContext)
        {
            context = dataContext;
        }
        public async Task<IActionResult> Index(long id = 1)
        {
            Product p = await context.Products.FindAsync(id);
            if (p.CategoryId == 1)
            {
                return View("Watersports", p);
            }
            else
            {
                return View(p);
            }
        }

        public IActionResult List()
        {
            return View(context.Products);
        }

        public IActionResult Common()
        {
            return View();
        }
    }
}
