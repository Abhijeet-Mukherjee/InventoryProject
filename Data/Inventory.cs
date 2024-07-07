using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvertoryProject.Data
{
    [Table("Inventory")]
    public class Inventory
    {
        [Key]
        public int inventoryId { get; set; }


        public int ProductId { get; set; }
        [ForeignKey("ProductId")]

        public Product? Product { get; set; }

        public int Quantity { get; set; }

        public string? WarehouseLocation { get; set; }


    }

}

