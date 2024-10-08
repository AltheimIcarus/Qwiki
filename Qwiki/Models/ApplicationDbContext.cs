﻿using Microsoft.EntityFrameworkCore;
using NodaTime;


// ToDo:
// In order to create Db, run: Add-Migration "Initial migration"
// To create table and update value: Update-Database

namespace Qwiki.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>().HasKey(e => e.Id);
            modelBuilder.Entity<Article>().HasIndex(e => e.Title).IsUnique();
            //e.g. modelBuilder.Entity<Super>().HasOne(p => p.child).WithMany(s => s.siblings).HasForeignKey();

            IClock clock = SystemClock.Instance;
            modelBuilder.Entity<Article>().HasData(
                new Article { Id = 1, Title = "First wiki page", Published = clock.GetCurrentInstant(), Thumbnail = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/06/Wikipedia-logo_ka.png/600px-Wikipedia-logo_ka.png?20150720233507", Content = "First landing wiki page in production." }
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
