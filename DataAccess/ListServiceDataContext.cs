namespace DataAccess
{
    using Microsoft.EntityFrameworkCore;

    public class ListServiceDataContext : DbContext
    {
        public ListServiceDataContext(DbContextOptions<ListServiceDataContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("ls");
        }
    }
}
