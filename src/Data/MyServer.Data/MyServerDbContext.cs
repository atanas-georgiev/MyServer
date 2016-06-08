namespace MyServer.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    using ImageGallery.Data.Models;

    using Microsoft.AspNet.Identity.EntityFramework;

    using MyServer.Data.Common.Models;

    public class MyServerDbContext : IdentityDbContext<User>
    {
        public MyServerDbContext()
            : base("MyServerDb", false)
        {
        }

        // public virtual IDbSet<Album> Albums { get; set; }

        // public virtual IDbSet<ImageGpsData> ImageGpsDatas { get; set; }

        // public virtual IDbSet<MediaTypeNames.Image> Images { get; set; }
        public static MyServerDbContext Create()
        {
            return new MyServerDbContext();
        }

        public override int SaveChanges()
        {
            this.ApplyAuditInfoRules();
            return base.SaveChanges();
        }

        private void ApplyAuditInfoRules()
        {
            // Approach via @julielerman: http://bit.ly/123661P
            foreach (var entry in
                this.ChangeTracker.Entries()
                    .Where(
                        e =>
                        e.Entity is IAuditInfo && ((e.State == EntityState.Added) || (e.State == EntityState.Modified)))
                )
            {
                var entity = (IAuditInfo)entry.Entity;
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