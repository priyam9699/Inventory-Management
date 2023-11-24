using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class ReturnIn
    {
        [Key]
        public int Id { get; set; }

        // Other properties for OrderOut

        [Required]
        public int ProductId { get; set; } // Foreign key

        [Required]
        public DateTime ReturnDate { get; set; }

        [Required]
        public string ProductSKU { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
