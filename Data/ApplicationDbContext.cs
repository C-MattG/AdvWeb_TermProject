using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HighlandTechSolutions.Models;

namespace HighlandTechSolutions.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //Define DbSet properrties for entities
        public DbSet<Service> Services { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentService> AppointmentServices { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // One-to-one relationship: A User can have one Review
            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithOne(u => u.Review) // A user has one review
                .HasForeignKey<Review>(r => r.UserId) // Foreign key is UserId in Review
                .OnDelete(DeleteBehavior.Cascade); // If the user is deleted, their review will be deleted too.

            // One-to-many relationship: User has many Quotes
            builder.Entity<Quote>()
                .HasOne(q => q.User)
                .WithMany(u => u.Quotes)
                .HasForeignKey(q => q.UserId);

            // One-to-many relationship: User has many Appointments
            builder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.UserId);

            // Many-to-many relationship: Appointment <-> Service (via AppointmentService)
            builder.Entity<AppointmentService>()
                .HasKey(a => new { a.AppointmentId, a.ServiceId });

            builder.Entity<AppointmentService>()
                .HasOne(a => a.Appointment)
                .WithMany(a => a.AppointmentServices)
                .HasForeignKey(a => a.AppointmentId);

            builder.Entity<AppointmentService>()
                .HasOne(a => a.Service)
                .WithMany()
                .HasForeignKey(a => a.ServiceId);
        }
    }
}
