﻿using GHPRS.Core.Entities;
using GHPRS.Persistence.Seed;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GHPRS.Persistence
{
    public class GhprsContext : IdentityDbContext<User>
    {
        public GhprsContext(DbContextOptions<GhprsContext> options) : base(options)
        {

        }

        public DbSet<Lookup> Lookups { get; set; }

        public DbSet<Template> Templates { get; set; }

        public DbSet<Upload> Uploads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(c => c.Uploads)
                .WithOne(e => e.User);

            modelBuilder.Seed();
        }
    }
}