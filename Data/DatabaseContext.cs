using Microsoft.EntityFrameworkCore;
using FindYourMusic.Models;

namespace FindYourMusic.Data {
    public class DatabaseContext : DbContext {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {
            
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.UseCollation("utf8mb4_unicode_ci");
            base.OnModelCreating(builder);
        }

        public DbSet<User> User { get; set; }
        public DbSet<CategoryGroup> CategoryGroup { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<UserTrackCategory> UserTrackCategory { get; set; }
        public DbSet<Track> Track { get; set; }
        public DbSet<Bookmark> Bookmark { get; set; }
    }
}
