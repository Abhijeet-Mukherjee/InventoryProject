using InvertoryProject.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryProject.Data
{
    public class Sale
    {
        public string Customer { get; set; }
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]

        public Product? Product { get; set; }
        public DateTime Timestamp { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalAmount { get; set; }
        // Add any other relevant properties
    }
}

