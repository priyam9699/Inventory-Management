using Microsoft.EntityFrameworkCore;
using InventoryManagement.Models;

namespace InventoryManagement
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {


        }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderOut> OrderOuts { get; set; }

        public DbSet<ReturnIn> ReturnIns { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //base.OnModelCreating(builder);

            //builder.Entity<OrderOut>()
            //.HasOne(o => o.Product)
            //.WithMany(p => p.OrderOuts)
            //.HasForeignKey(o => o.ProductId);

        }


        

    }
}
