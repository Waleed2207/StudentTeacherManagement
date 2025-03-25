using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentTeacherManagement.Models;
using StudentTeacherManagement.Models.Identity;
using Assignment = StudentTeacherManagement.Models.Entities.Assignment;
using Submission = StudentTeacherManagement.Models.Entities.Submission;

namespace StudentTeacherManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Submission> Submissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Explicit mapping for clarity (optional)
            builder.Entity<ApplicationUser>().ToTable("Users");

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Teacher", NormalizedName = "TEACHER" },
                new IdentityRole { Id = "2", Name = "Student", NormalizedName = "STUDENT" },
                new IdentityRole { Id = "3", Name = "Admin", NormalizedName = "ADMIN" }

            );

            builder.Entity<Assignment>()
                .HasOne(a => a.Teacher)
                .WithMany()
                .HasForeignKey(a => a.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Submission>()
                .HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }


}