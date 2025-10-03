using Microsoft.EntityFrameworkCore;
using PeerCraft.Models;

namespace PeerCraft.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Defining the database tables
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Technology> Technologies { get; set; }
        public DbSet<ProjectTechnology> ProjectTechnologies { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reply> Replies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Setting up the relationship between Projects and Users (Owner)
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.OwnedProjects)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict); // So deleting a user doesn't delete their projects

            // Setting up the relationships for Team Members
            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Project)
                .WithMany(p => p.TeamMembers)
                .HasForeignKey(tm => tm.ProjectId);

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.User)
                .WithMany(u => u.TeamMemberships)
                .HasForeignKey(tm => tm.UserId);

            // Setting up the many-to-many relationship between Projects and Technologies
            modelBuilder.Entity<ProjectTechnology>()
                .HasKey(pt => new { pt.ProjectId, pt.TechnologyId }); // Composite key

            modelBuilder.Entity<ProjectTechnology>()
                .HasOne(pt => pt.Project)
                .WithMany(p => p.ProjectTechnologies)
                .HasForeignKey(pt => pt.ProjectId);

            modelBuilder.Entity<ProjectTechnology>()
                .HasOne(pt => pt.Technology)
                .WithMany(t => t.ProjectTechnologies)
                .HasForeignKey(pt => pt.TechnologyId);

            // Setting up relationships for Comments and Replies
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reply>()
                .HasOne(r => r.Author)
                .WithMany()
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Adding some initial data for the Technologies table
            modelBuilder.Entity<Technology>().HasData(
                new Technology { Id = 1, Name = "C#" },
                new Technology { Id = 2, Name = "ASP.NET Core" },
                new Technology { Id = 3, Name = "React" },
                new Technology { Id = 4, Name = "Python" },
                new Technology { Id = 5, Name = "Bootstrap" },
                new Technology { Id = 6, Name = "JavaScript" },
                new Technology { Id = 7, Name = "Java" },
                new Technology { Id = 8, Name = "SQL Server" }
            );
        }
    }
}