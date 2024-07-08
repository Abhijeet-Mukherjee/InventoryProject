using InvertoryProject.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace InventoryProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Product> products { get; set; }

        public DbSet<Inventory> inventory { get; set; }

        public DbSet<Sale> Sales { get; set; }

    }
}
