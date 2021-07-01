using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApp.Controllers
{
    // p697
    [AutoValidateAntiforgeryToken]
    public class FormController : Controller    // p672
    {
        private DataContext context;

        public FormController(DataContext dbContext)
        {
            context = dbContext;
        }

        public async Task<IActionResult> Index([FromQuery] long? id)
        {
            // p685
            ViewBag.Categories = new SelectList(context.Categories,"CategoryId","Name");    // p692
            return View("Form", await context.Products.FirstOrDefaultAsync(p => p.ProductId == id));
            //return View("Form", await context.Products
            //    .Include(p => p.Category)
            //    .Include(p => p.Supplier)
            //    .FirstOrDefaultAsync(p => id == null || p.ProductId == id));
            //.FirstOrDefaultAsync(p => p.ProductId == id));   // p711
        }

        [HttpPost]
        public IActionResult SubmitForm(Product product)
        {
         // p743 had model validation coded in controller - not good
         // Removed in p763 to allow Custom validation to be applied to Model and its Properties - see classes defined in Validation folder, then applied to Product.cs

            if (ModelState.IsValid) // Validation attributes for Product are applied BEFORE the action method is called (p760&763)
            {
                TempData["name"] = product.Name;
                TempData["price"] = product.Price.ToString();
                TempData["categoryId"] = product.CategoryId.ToString();
                TempData["supplierId"] = product.SupplierId.ToString();
                return RedirectToAction(nameof(Results));
            }
            else
            {
                return View("Form");
            }
        }

        public IActionResult Results()
        {
            return View(TempData);
        }

        public string Header([FromHeader(Name = "Accept-Language")]string accept)
        {
            return $"Header: {accept}";
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]    // because to test I'm not using a token
        public Product Body([FromBody] Product model)
        {
            return model;
        }
    }
}