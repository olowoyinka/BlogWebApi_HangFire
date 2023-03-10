using BlogWebApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApi.Infrastructure.Persistence
{
    public class BlogDbContext : IdentityDbContext<User>
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(BlogDbContext).Assembly);

            builder.Entity<User>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");


            // Unique Key
            builder.Entity<User>()
               .HasIndex(bc => new { bc.PhoneNumber })
               .IsUnique();


            // Relationship
            builder.Entity<Comment>()
                .HasOne(bc => bc.Blog)
                .WithMany(b => b.Comments)
                .HasForeignKey(bc => bc.BlogId);


            // Soft Delete
            builder.Entity<User>()
                .HasQueryFilter(s => !s.IsDeleted);

            builder.Entity<Blog>()
                .HasQueryFilter(s => !s.IsDeleted);

            builder.Entity<Tag>()
                .HasQueryFilter(s => !s.IsDeleted);

            builder.Entity<Comment>()
                .HasQueryFilter(s => !s.IsDeleted);

            // Indexes
            builder.Entity<Blog>()
                .HasIndex(s => new { s.Title });
        }

        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
    }
}