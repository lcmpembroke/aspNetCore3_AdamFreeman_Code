using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Validation;

namespace WebApp.Models
{
    [PhraseAndPrice(Phrase = "Small", Price = "100")] // p762: Model-level custom validation defined in Validation/PhraseAndPriceAttribute.cs (extends ValidationAttribute)
    public class Product
    {
        public long ProductId { get; set; }
        
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Column(TypeName = "decimal(8,2)")]                                         // p429
        [Required(ErrorMessage = "Please enter a price")]                           // p758
        [Range(1, 999999, ErrorMessage = "Please enter a positive price")]          // p758
        //[DisplayFormat(DataFormatString = "{0:c2}",ApplyFormatInEditMode = true)]   // p684 & p704
        public decimal Price { get; set; }

        [PrimaryKey(ContextType = typeof(DataContext), DataType = typeof(Category))]    // p763: Property-level custom validation defined in Validation/PrimaryKeyAttribute.cs (extends ValidationAttribute)
        [Remote("CategoryKey","Validation", ErrorMessage = "Enter an existing category key")]   // p769 remote validation being applied
        public long CategoryId { get; set; }
        public Category Category { get; set; }

        [PrimaryKey(ContextType = typeof(DataContext), DataType = typeof(Supplier))]    // p762: Property-level custom validation defined in Validation/PrimaryKeyAttribute.cs (extends ValidationAttribute)
        [Remote("SupplierKey", "Validation", ErrorMessage = "Enter an existing supplier key")]   // p769 remote validation being applied --> configures unobtrusive validation using Ajax to send Http request to see if valid, and returns Json boolean
        public long SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }
}
