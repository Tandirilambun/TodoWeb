using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoWeb.Models;

namespace TodoWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

        public DbSet<Category> Category { get; set; }
        public DbSet<Todo> Todos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Projects> Projects { get; set; }
        public DbSet<Participants> Participants { get; set; }
        public DbSet<Tasks> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Participants>()
                .HasKey(part => new {part.ProjectId, part.UserId});
            builder.Entity<Participants>()
                .HasOne(part => part.Projects)
                .WithMany(p => p.Participants)
                .HasForeignKey(p => p.ProjectId);
            builder.Entity<Participants>()
                .HasOne(part => part.User)
                .WithMany(p => p.Participants)
                .HasForeignKey(p => p.UserId);
        }

    }
}
