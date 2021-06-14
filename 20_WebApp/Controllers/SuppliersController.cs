using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : Controller
    {
        private DataContext context;
        public SuppliersController(DataContext dataContext)
        {
            context = dataContext;
        }

        [HttpGet("{id}")]
        public async Task<Supplier> GetSupplier(long id)
        {
            // here, related data not brought in
            // return await context.Suppliers.FindAsync(id);

            // here, related data brought in
            // Note p473: need to break the circular reference in the related data or it causes a JsonException
                Supplier supplier = await context.Suppliers
                .Include(s => s.Products)
                .FirstAsync(s => s.SupplierId == id);

            foreach (Product p in supplier.Products)
            {
                p.Supplier = null;
            }
            return supplier;
        }

        [HttpPatch("{id}")]
        public async Task<Supplier> PatchSupplier(long id, JsonPatchDocument<Supplier> patchDoc)
        {
            Supplier s = await context.Suppliers.FindAsync(id);
            if (s != null)
            {
                patchDoc.ApplyTo(s);
                await context.SaveChangesAsync();
            }
            return s;
        }
    }
}
