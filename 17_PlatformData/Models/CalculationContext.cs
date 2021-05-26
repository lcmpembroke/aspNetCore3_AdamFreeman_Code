using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Platform.Models
{
    public class CalculationContext : DbContext 
    {
        public CalculationContext(DbContextOptions<CalculationContext> opts) : base(opts) { 
        }

        public DbSet<Calculation> Calculations{ get; set; }
    }
}
