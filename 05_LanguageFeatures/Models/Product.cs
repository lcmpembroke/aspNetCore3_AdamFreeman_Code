using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageFeatures.Models
{
    public class Product
    {
        public string Name { get; set; }
        public string Category { get; set; } = "Watersports";   // auto-implemented property initialiser (can be chnged later)
        public decimal? Price { get; set; }
        public Product Related { get; set; }
        public bool InStock { get; }    // this is read-only so can only be assigned differently in the contructor, after that it's set.
        public bool NameBeginsWithS => Name?[0] == 'S';     // BOOK p93 a Lambda property (essentially a computed property)

        public Product(bool stock = true)
        {
            InStock = stock;
        }
        public static Product[] GetProducts()
        {
            // create Products using obkect initialiser syntax
            Product kayak = new Product { Name = "Kayak", Category = "Water Craft", Price = 275M };
            Product lifejacket = new Product(false) { Name = "Lifejacket", Price = 48.95M };
            kayak.Related = lifejacket;
            return new Product[] { kayak, lifejacket, null };
        }
    }
}
