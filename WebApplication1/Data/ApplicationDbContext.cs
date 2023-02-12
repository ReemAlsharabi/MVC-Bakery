using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        {
        }

        public ApplicationDbContext()
        {
        }

        public DbSet<Category> Categories { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=LAPTOP-I2MMCIE7;Database=Bakery;Trusted_Connection=True;Encrypt=false;");
        }
        public DbSet<WebApplication1.Models.Customer> Customer { get; set; }
        public DbSet<WebApplication1.Models.Cart> Cart { get; set; }
        public DbSet<WebApplication1.Models.Order> Order { get; set; }
        public DbSet<WebApplication1.Models.OrderItem> OrderItem { get; set; }
        public DbSet<WebApplication1.Models.Payment> Payment { get; set; }
        public DbSet<WebApplication1.Models.Product> Product { get; set; }
    }
}
