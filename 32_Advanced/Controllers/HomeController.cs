using Advanced.Models;
using Advanced.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advanced.Controllers
{
    public class HomeController : Controller
    {
        private DataContext context;

        public HomeController(DataContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index([FromQuery] string selectedCity)
        {
            PeopleListViewModel vm = new PeopleListViewModel {
                People = context.People
                .Include(p => p.Department)
                .Include(p => p.Location
                
                ),
                Cities = context.Locations.Select(l => l.City).Distinct(),
                SelectedCity = selectedCity

            };
            return View(vm);
        }
    }

}
