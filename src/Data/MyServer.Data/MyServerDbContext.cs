namespace MyServer.Data
{
    using System;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    using MyServer.Data.Common.Models;
    using MyServer.Data.Models;

    public class MyServerDbContext : IdentityDbContext<User>
    {
        public MyServerDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<Album> Albums { get; set; }

        public virtual DbSet<Comment> Comments { get; set; }

        public virtual DbSet<ImageGpsData> ImageGpsDatas { get; set; }

        public virtual DbSet<Image> Images { get; set; }

        public override int SaveChanges()
        {
            this.ApplyAuditInfoRules();
            return base.SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Album>().HasOne(p => p.Cover).WithMany(b => b.Covers);
            modelBuilder.Entity<Image>().HasMany(x => x.Covers).WithOne(x => x.Cover).HasForeignKey(x => x.CoverId);
            base.OnModelCreating(modelBuilder);
        }

        private void ApplyAuditInfoRules()
        {
            foreach (var entry in this.ChangeTracker.Entries())
            {
                if (entry.Entity is IAuditInfo
                    && ((entry.State == EntityState.Added) || (entry.State == EntityState.Modified)))
                {
                    var entity = entry.Entity as IAuditInfo;
                    if (entry.State == EntityState.Added && entity.CreatedOn == default(DateTime))
                    {
                        entity.CreatedOn = DateTime.Now;
                    }
                    else
                    {
                        entity.ModifiedOn = DateTime.Now;
                    }
                }
            }
        }
    }
}