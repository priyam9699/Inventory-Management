using InventoryManagement.IRepository;
using InventoryManagement.Models;

namespace InventoryManagement.SqlRepository
{
    public class SqlOrderOutRepository : IOrderOutRepository
    {
        private readonly ApplicationDbContext context;

        public SqlOrderOutRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public OrderOut Add(OrderOut orderOut)
        {
            context.OrderOuts.Add(orderOut);
            context.SaveChanges();
            return orderOut;
        }

        public OrderOut Delete(int id)
        {
            var orderout = context.OrderOuts.Find(id);
            if (orderout != null)
            {
                context.OrderOuts.Remove(orderout);
                context.SaveChanges();
            }
            return orderout;
        }

        public IEnumerable<OrderOut> GetAllOrderOut()
        {
            return context.OrderOuts.ToList();
        }

        public OrderOut GetById(int id)
        {
            return context.OrderOuts.Find(id);
        }

        public IEnumerable<OrderOut> SearchOrderOut(string search)
        {
            return context.OrderOuts
        .Where(p => p.ProductSKU.Contains(search))
        .ToList();
        }

        public OrderOut Update(OrderOut orderOut)
        {
            var orderout = context.OrderOuts.Attach(orderOut);
            orderout.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return orderOut;
        }
    }
}
