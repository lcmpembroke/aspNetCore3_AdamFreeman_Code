using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.Linq;
using WebApp.Models;

namespace WebApp.Components
{
    public class CitySummary : ViewComponent
    {
        private CitiesData data;

        public CitySummary(CitiesData citiesData)
        {
            data = citiesData;
        }

        // page 589 just returns a string & p599 to show use of context data
        //public string Invoke()
        //{
        //    // return $"{data.Cities.Count()} cities, " + $"{data.Cities.Sum(c => c.Population)} people";

        //    if (RouteData.Values["controller"] != null)
        //    {
        //        return $"CONTROLLER REQUEST: {data.Cities.Count()} cities, " + $"{data.Cities.Sum(c => c.Population)} people";
        //    }
        //    else
        //    {
        //        return $"Razor PAGE REQUEST: {data.Cities.Count()} cities, " + $"{data.Cities.Sum(c => c.Population)} people";
        //    }
        //}

        // page 594 to use View() method, needs a View file (WebApp\Views\Shared\Components\CitySummary\Default.cshtml) to return an IViewComponentResult
        //public IViewComponentResult Invoke()
        //{
        //    return View(new CityViewModel { 
        //        Cities = data.Cities.Count(), 
        //        Population = data.Cities.Sum(c => c.Population) 
        //    });
        //}

        // page 597 to use Content() method which ENCODES the string to return an IViewComponentResult - no view needed, just takes in a string html fragment
        //public IViewComponentResult Invoke()
        //{
        //    // this will just be a string not HTML on the page
        //    return Content("<h3>This is a <i>string</i> from Views\\Shared\\Components\\CitySummary\\Default.cshtml Invoke() method </h3>");
        //}

        // page 598 to use HtmlContentViewComponentResult() method to return an IViewComponentResult - no view needed, just takes in a string html fragment
        //public IViewComponentResult Invoke()
        //{
        //    // this will just be a string not HTML on the page
        //    return new HtmlContentViewComponentResult(new HtmlString("<h6>This is a <i>string</i> from Views\\Shared\\Components\\CitySummary\\Default.cshtml Invoke() method </h6>"));
        //}

        // p600 providing context from parent view
        public IViewComponentResult Invoke(string themeName)
        {
            ViewBag.Theme = themeName;

            return View(new CityViewModel
            {
                Cities = data.Cities.Count(),
                Population = data.Cities.Sum(c => c.Population)
            });
        }

    }
}
