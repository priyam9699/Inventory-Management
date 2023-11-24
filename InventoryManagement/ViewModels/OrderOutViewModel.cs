using InventoryManagement.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.ViewModels
{
    public class OrderOutViewModel
    {
        [Key]
        public int Id { get; set; }

        // Other properties for OrderOut

        [Required]
        public int ProductId { get; set; } // Foreign key

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string ProductSKU { get; set; }

        [Required]
        public int Quantity { get; set; }

        // Navigation property for the related Product
        //[ForeignKey("ProductId")]
        //public Product Product { get; set; }
    }
}
