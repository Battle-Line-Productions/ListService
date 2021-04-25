namespace ListService.Service
{
    using Contracts.Domain.V1;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AppList> AppLists { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<AppListItem> ListItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppListItem>().Ignore(xx => xx.AppList).HasKey(x => new {x.AppListId, x.ItemName});
        }
    }
}