using InventoryManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.ViewModels
{
    public class ProductViewModel
    {
		[Key]
		public int Id { get; set; }
		[Required]
		public DateTime Date { get; set; }
		[Required]
		public string ProductName { get; set; }
		[Required]
		public string SKU { get; set; }
		[Required]
		public int Price { get; set; }

		[Required]
		public int Quantity { get; set; }

		public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
