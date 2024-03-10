using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QrSystem.DAL
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=SQL5109.site4now.net;Database=db_aa642d_aztukaf;User Id=db_aa642d_aztukaf_admin;Password=melikov__03;");

            return new AppDbContext(optionsBuilder.Options);
            //}
        }
    }
}
