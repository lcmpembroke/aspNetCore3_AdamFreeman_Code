using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Validation
{
    public class PhraseAndPriceAttribute : ValidationAttribute
    {
        public string Phrase { get; set; }
        public string Price { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Product product = value as Product;

            if (product != null 
                && product.Name.StartsWith(Phrase, StringComparison.OrdinalIgnoreCase)
                && product.Price > decimal.Parse(Price))
            {
                return new ValidationResult(ErrorMessage ?? $"PhraseAndPrice attribute error: {Phrase} products cannot cost more than £{Price}");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}
