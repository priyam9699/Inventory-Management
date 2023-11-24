using InventoryManagement.IRepository;
using InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace InventoryManagement.SqlRepository
{
	public class SqlProductRepository : IProductRepository
	{
		private readonly ApplicationDbContext context;

		public SqlProductRepository(ApplicationDbContext context)
		{
			this.context = context;
		}

		public Product Add(Product product)
		{
			context.Products.Add(product);
			context.SaveChanges();
			return product;
		}

		public Product Delete(int id)
		{
			var product = context.Products.Find(id);
			if (product != null)
			{
				context.Products.Remove(product);
				context.SaveChanges();
			}
			return product;
		}

		public IEnumerable<Product> GetAllProduct()
		{
			return context.Products.ToList();
		}



		public Product GetById(int id)
		{
			return context.Products.Find(id);
		}

        public IEnumerable<Product> SearchProducts(string search)
        {
            return context.Products
        .Where(p => p.ProductName.Contains(search) || p.SKU.Contains(search))
        .ToList();
        }

        public Product Update(Product UpdateProduct)
		{
			var product = context.Products.Attach(UpdateProduct);
			product.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
			context.SaveChanges();
			return UpdateProduct;
				
		}
	}
}
