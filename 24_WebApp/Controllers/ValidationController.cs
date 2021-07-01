using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValidationController : ControllerBase
    {
        private DataContext dataContext;

        public ValidationController(DataContext dbContext)
        {
            dataContext = dbContext;
        }

        [HttpGet("categorykey")]
        public bool CategoryKey(string categoryId, [FromQuery] KeyTarget target)      // p768 & 770
        {
            long keyVal;
            return long.TryParse(categoryId ?? target.CategoryId, out keyVal)
                && dataContext.Categories.Find(keyVal) != null;
        }

        [HttpGet("supplierkey")]
        public bool SupplierKey(string supplierId, [FromQuery] KeyTarget target)          // p768 & 770
        {
            long keyVal;
            return long.TryParse(supplierId ?? target.CategoryId, out keyVal)
                && dataContext.Suppliers.Find(keyVal) != null;
        }

        [Bind(Prefix = "Product")]  // to enable validation to work both with MVC and Razor pages - see p770
        public class KeyTarget
        {
            public string CategoryId { get; set; }
            public string SupplierId { get; set; }
        }

    }

}
