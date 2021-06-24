using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;

namespace WebApp.Pages
{

    public class IndexModel : PageModel
    {
        private DataContext context;

        public Product Product { get; set; }

        public IndexModel(DataContext dataContext)
        {
            context = dataContext;
        }

        public async Task<IActionResult> OnGet(long id = 2)
        {
            Product = await context.Products.FindAsync(id);
            if (Product == null)
            {
                return RedirectToPage("NotFound");
            }
            return Page();
        }
    }
}
