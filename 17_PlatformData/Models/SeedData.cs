using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Platform.Models
{
    public class SeedData
    {

        private CalculationContext context;
        private ILogger<SeedData> logger;
        private static Dictionary<int, long> data = new Dictionary<int, long>() {
            {1,1}, {2,3}, {3,6}, {4,10}, {5,15}, {6,21}, {7,28}, {8,36}, {9,45}, {10,55}
        };

        public SeedData(CalculationContext dataContext, ILogger<SeedData> log)
        {
            context = dataContext;
            logger = log;
        }

        public void SeedDatabase()
        {
            context.Database.Migrate();
            if (context.Calculations.Count() == 0)
            {
                logger.LogInformation("Preparing to seed database");

                // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-forgiving
                // ! is the null forgiving operator


                // addRacnge accepts a sequence of calculation objects
                context.Calculations!.AddRange(data.Select(kvp => new Calculation() { Count = kvp.Key, Result = kvp.Value }));
                context.SaveChanges();
                logger.LogInformation("Database seeded");
            }
            else
            {
                logger.LogInformation("Database not seeded");
            }

        }
    }
}
