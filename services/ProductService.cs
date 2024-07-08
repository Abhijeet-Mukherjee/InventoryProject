using InventoryProject.Data;

namespace InventoryProject.services
{
    public interface IProductService
    {
        Task<bool> UpdateStock(int productId, int quantitySold);
    }

    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UpdateStock(int productId, int quantitySold)
        {
            var product = await _context.products.FindAsync(productId);

            if (product == null)
            {
                return false; // Product not found
            }

            if (product.Price < quantitySold)
            {
                return false; // Insufficient stock
            }

            product.Price -= quantitySold;
            await _context.SaveChangesAsync();

            return true;
        }
    }

}
