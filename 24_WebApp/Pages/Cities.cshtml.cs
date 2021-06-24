using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using WebApp.Models;

namespace WebApp.Pages
{
    
    [ViewComponent(Name = "CitiesPageHybrid")]      // p605
    public class CitiesModel : PageModel
    {
        
        public CitiesModel(CitiesData citiesData)
        {
            Data = citiesData;
        }

        public CitiesData Data { get; set; }

        [ViewComponentContext]
        public ViewComponentContext Context { get; set; }

        public IViewComponentResult Invoke()
        {
            return new ViewViewComponentResult() { ViewData = new ViewDataDictionary<CityViewModel>( 
                    Context.ViewData,
                    new CityViewModel { Cities = Data.Cities.Count(), Population = Data.Cities.Sum(c => c.Population) }
                ) };
        }

    }
}
