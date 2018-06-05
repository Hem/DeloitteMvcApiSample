using System.Data.Entity;

namespace DeloitteMvcApiSample.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<ApplicationDbContext>());
        }
        
        public DbSet<UserTokenCache> UserTokenCacheList { get; set; }
    }
}
