using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace WebApp.Models
{
    public class Supplier
    {
        public long SupplierId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string City { get; set; }

        public IEnumerable<Product> Products { get; set; }
    }
}
