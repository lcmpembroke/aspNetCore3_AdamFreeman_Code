using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ContentController : ControllerBase
    {
        private DataContext context;

        public ContentController(DataContext dataContext)
        {
            context = dataContext;
        }

        [HttpGet("string")]
        public string GetString() => "This is a string response from the ContentController with route /api/Content/string-";

        [HttpGet("object/{format?}")]   // p483 optional format segment variable - it overrides Accept header sent byb client
        [FormatFilter]                  // p483 uses format value in route data or querystring to set content type on ObjectResult returned from this action method
        [Produces("application/json","application/xml")]
        public async Task<ProductBindingTarget> GetObject()
        {
            Product p = await context.Products.FirstAsync();
            return new ProductBindingTarget() {
                Name = p.Name, Price = p.Price, CategoryId = p.CategoryId, SupplierId = p.SupplierId
            };
        }

        [HttpPost]
        [Consumes("application/json")]
        public string SaveProductJson(ProductBindingTarget product)
        {
            return $"JSON was consumed: {product.Name}";
        }

        // Commented out to allow for OpenAPI spec (Swagger) - unique (HttbVerb + URL pattern)
        //[HttpPost]
        //[Consumes("application/xml")]
        //public string SaveProductXml(ProductBindingTarget product)
        //{
        //    return $"XML was consumed: {product.Name}";
        //}

    }
}
