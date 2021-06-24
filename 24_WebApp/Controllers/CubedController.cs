using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public class CubedController : Controller
    {
        public IActionResult Index()
        {
            return View("Cubed");
        }

        public IActionResult Cube(double num,double num2)
        {
            // commented out Tempdata dictionary below to show use of [TempData] attribute as an equivalent alternative
            // use of TemData dictionary might be easier to follow as shows intent of the action
            //TempData["value"] = num.ToString();
            //TempData["result"] = Math.Pow(num,3).ToString();
            Value = num.ToString();
            Result = Math.Pow(num, 3).ToString();
            return RedirectToAction(nameof(Index));
        }

        [TempData]
        public string Value { get; set; }

        [TempData]
        public string Result { get; set; }
    }
}
