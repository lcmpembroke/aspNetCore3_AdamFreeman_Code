using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Platform
{
    public class MessageOptions
    {
        [Required]
        public string CityName { get; set; } = "Durham";
        public string CountryName { get; set; } = "ENGLAND";
    }
}
