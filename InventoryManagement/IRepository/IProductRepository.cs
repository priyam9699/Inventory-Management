using InventoryManagement.Models;

namespace InventoryManagement.IRepository
{
	public interface IProductRepository
	{
		Product GetById(int id);

		IEnumerable<Product> GetAllProduct();
		Product Add(Product Product);
		Product Update(Product UpdateProduct);
		Product Delete(int id);
        IEnumerable<Product> SearchProducts(string search);
    }
}
