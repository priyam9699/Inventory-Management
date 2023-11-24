using InventoryManagement.Models;

namespace InventoryManagement.IRepository
{
    public interface IReturnInRepository
    {
        ReturnIn GetById(int id);

        IEnumerable<ReturnIn> GetAllReturnIn();
        ReturnIn Add(ReturnIn ReturnIn);
        ReturnIn Update(ReturnIn UpdateReturnIn);
        ReturnIn Delete(int id);
        IEnumerable<ReturnIn> SearchReturnIn(string search);
    }
}
