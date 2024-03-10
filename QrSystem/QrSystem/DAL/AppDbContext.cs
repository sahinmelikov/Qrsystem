using Microsoft.EntityFrameworkCore;
using QrSystem.Models;


namespace QrSystem.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<RestourantTables> Tables { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<QrCode> QrCodes { get; set; }
        public DbSet<SaxlanilanSifarish> SaxlanilanS { get; set; }
        public DbSet<ParentsCategory> ParentsCategories { get; set; }
    }
}
