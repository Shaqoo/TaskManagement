using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.DBaseContext
{
    using Domain.Entities;
    using Domain.ValueObjects;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<AppLog> ApplicationLogs => Set<AppLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var emailConverter = new ValueConverter<Email, string>(
                v => v.Value,
                v => new Email(v)
            );
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("App_Users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).HasConversion(emailConverter).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.Email);
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);
                entity.Property(u => u.CreatedAt).IsRequired();
            });
          

            modelBuilder.Entity<AppLog>().ToTable("ApplicationLogs");
        }
    }

}
