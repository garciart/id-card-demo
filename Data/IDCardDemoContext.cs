using Microsoft.EntityFrameworkCore;

namespace IDCardDemo.Data
{
    public class IDCardDemoContext : DbContext
    {
        public IDCardDemoContext(
            DbContextOptions<IDCardDemoContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Holder> Holder { get; set; }
    }
}