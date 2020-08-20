﻿using Microsoft.EntityFrameworkCore;

namespace NeverFoundry.Wiki.Samples.Complete.Data
{
    public class IdentityDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<WikiUser>
    {
        public DbSet<FidoStoredCredential> FidoStoredCredential { get; set; } = null!;

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<FidoStoredCredential>().HasKey(m => m.Username);
            base.OnModelCreating(builder);
        }
    }
}
