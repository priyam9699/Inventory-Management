using InventoryManagement.Models;

namespace InventoryManagement.IRepository
{
    public interface IOrderOutRepository
    {
        OrderOut GetById(int id);

        IEnumerable<OrderOut> GetAllOrderOut();
        OrderOut Add(OrderOut orderOut);
        OrderOut Update(OrderOut orderOut);
        OrderOut Delete(int id);
        IEnumerable<OrderOut> SearchOrderOut(string search);
    }
}
