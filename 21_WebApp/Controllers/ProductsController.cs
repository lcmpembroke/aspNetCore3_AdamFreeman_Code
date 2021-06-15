using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using WebApp.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApp.Controllers
{
    [ApiController]
    // Route attribute for controller specifies the URL for the controller (drop controller part of name) i.e. "api/products" here
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private DataContext context;
        public ProductsController(DataContext dataContext)
        {
            context = dataContext;
        }


        [HttpGet]
        public IAsyncEnumerable<Product> GetProducts()
        {
            return context.Products;
        }

        //  ----------------- below is without returning an IActionResult interface -----------------------------
        //[HttpGet("{id}")]
        //public async Task<Product> GetProduct(long id, [FromServices] ILogger<ProductsController> logger)
        //{
        //    logger.LogDebug("GetProduct action invoked");
        //    return await context.Products.FindAsync(id);
        //}

        //// Invoked through the following command in Powershell:
        ////Invoke-RestMethod http://localhost:5000/api/products -Method POST -Body (@{ Name="Soccer Boots";Price=89.99; CategoryId=2; SupplierId=2} | ConvertTo-Json) -ContentType "application/json"
        //[HttpPost]
        //public async Task SaveProduct([FromBody] ProductBindingTarget target)
        //{
        //    await context.Products.AddAsync(target.ToProduct());
        //    await context.SaveChangesAsync();
        //}
        //  ----------------- above is without returning an IActionResult interface -----------------------------

        // below is using Action Results:
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct(long id, [FromServices] ILogger<ProductsController> logger)
        {
            logger.LogDebug("GetProduct action invoked");
            Product p = await context.Products.FindAsync(id);
            if (p == null)
            {
                return NotFound();
            }
            return Ok(p);
        }


        // Invoked through the following command in Powershell:
        //Invoke-RestMethod http://localhost:5000/api/products -Method POST -Body (@{ Name="Soccer Boots";Price=89.99; CategoryId=2; SupplierId=2} | ConvertTo-Json) -ContentType "application/json"
        [HttpPost]
        public async Task<IActionResult> SaveProduct([FromBody] ProductBindingTarget target)
        {
                Product p = target.ToProduct();
                await context.Products.AddAsync(p);
                await context.SaveChangesAsync();
                return Ok(p);
        }

        // below is before the [ApiController] attribute at top of class was added - which takes care of [FromBody] and checking ModelState automatically by MVC framework as they are so common]
        //[HttpPost]
        //public async Task<IActionResult> SaveProduct([FromBody] ProductBindingTarget target)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Product p = target.ToProduct();
        //        await context.Products.AddAsync(p);
        //        await context.SaveChangesAsync();
        //        return Ok(p);
        //    }
        //    return BadRequest(ModelState);
        //}

        // Invoked through the following command in Powershell:
        //Invoke-RestMethod http://localhost:5000/api/products -Method PUT -Body (@{ ProductId=1; Name="Green Kayak";Price=275; CategoryId=1; SupplierId=1} | ConvertTo-Json) -ContentType "application/json"
        [HttpPut]
        public async Task UpdateProduct(Product product)
        {
            context.Products.Update(product);
            await context.SaveChangesAsync();
        }

        // Invoked through the following command in Powershell:
        //Invoke-RestMethod http://localhost:5000/api/products/2 -Method DELETE
        [HttpDelete("{id}")]
        public async Task DeleteProduct(long id)
        {
            context.Products.Remove(new Product { ProductId = id });
            await context.SaveChangesAsync();
        }

        // /api/Products/redirect
        [HttpGet("redirect")]
        public IActionResult Redirect()
        {
            //return Redirect("/api/products/1");

            // parameters are ActionName, route values object
            return RedirectToAction(nameof(GetProduct), new { Id = 1 });
        }
    }
}
