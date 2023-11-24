using InventoryManagement.IRepository;
using InventoryManagement.Models;

namespace InventoryManagement.SqlRepository
{
    public class SqlReturnInRepository : IReturnInRepository
    {
        private readonly ApplicationDbContext context;

        public SqlReturnInRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public ReturnIn Add(ReturnIn ReturnIn)
        {
            context.ReturnIns.Add(ReturnIn);
            context.SaveChanges();
            return ReturnIn;
        }

        public ReturnIn Delete(int id)
        {
            var returnin = context.ReturnIns.Find(id);
            if (returnin != null)
            {
                context.ReturnIns.Remove(returnin);
                context.SaveChanges();
            }
            return returnin;
        }

        public IEnumerable<ReturnIn> GetAllReturnIn()
        {
            return context.ReturnIns.ToList();
        }

        public ReturnIn GetById(int id)
        {
            return context.ReturnIns.Find(id);
        }

        public IEnumerable<ReturnIn> SearchReturnIn(string search)
        {
            return context.ReturnIns
        .Where(p => p.ProductSKU.Contains(search))
        .ToList();
        }

        public ReturnIn Update(ReturnIn UpdateReturnIn)
        {
            var returnin = context.ReturnIns.Attach(UpdateReturnIn);
            returnin.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return UpdateReturnIn;
        }
    }
}
