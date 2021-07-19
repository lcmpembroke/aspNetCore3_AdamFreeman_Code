using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Validation;

namespace WebApp.Models
{
    public class Product
    {
        public long ProductId { get; set; }
        
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Column(TypeName = "decimal(8,2)")]                                         // p429
        [Required(ErrorMessage = "Please enter a price")]                           // p758
        [Range(1, 999999, ErrorMessage = "Please enter a positive price")]          // p758
        public decimal Price { get; set; }

        [PrimaryKey(ContextType = typeof(DataContext), DataType = typeof(Category))]    // p763: Property-level custom validation defined in Validation/PrimaryKeyAttribute.cs (extends ValidationAttribute)
        public long CategoryId { get; set; }
        public Category Category { get; set; }

        [PrimaryKey(ContextType = typeof(DataContext), DataType = typeof(Supplier))]    // p762: Property-level custom validation defined in Validation/PrimaryKeyAttribute.cs (extends ValidationAttribute)
        public long SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }
}
