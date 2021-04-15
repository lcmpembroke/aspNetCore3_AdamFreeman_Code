using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class EFOrderRepository : IOrderRepository
    {
        private StoreDbContext context;

        public EFOrderRepository(StoreDbContext injectedContext)
        {
            context = injectedContext;
        }

        // when an order is read from db, the collection associated with the Lines property should be loaded along with each Product object associated with each line in the collection
        public IQueryable<Order> Orders => context.Orders
                                            .Include(o => o.Lines)
                                            .ThenInclude(l => l.Product);

        public void SaveOrder(Order order)
        {
            // AttachRange notifies EFCore that the product objects already exist and shouldn't be stored in database unless they are modified (which they won't be here as user not able to modify products)
            context.AttachRange(order.Lines.Select(l => l.Product));
            if (order.OrderID == 0)
            {
                context.Orders.Add(order);
            }
            context.SaveChanges();
        }
    }
}
