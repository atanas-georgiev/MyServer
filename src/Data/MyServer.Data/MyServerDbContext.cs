namespace MyServer.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Data.SqlClient;
    using System.Linq;

    using ImageGallery.Data.Models;

    using Microsoft.AspNet.Identity.EntityFramework;

    using MyServer.Data.Common.Models;
    using MyServer.Data.Models.ImageGallery;

    public class MyServerDbContext : IdentityDbContext<User>
    {
        private static Dictionary<Type, EntitySetBase> _mappingCache = new Dictionary<Type, EntitySetBase>();

        public MyServerDbContext()
            : base("MyServerDb", false)
        {
        }

        public virtual IDbSet<Album> Albums { get; set; }

        public virtual IDbSet<ImageGpsData> ImageGpsDatas { get; set; }

        public virtual IDbSet<Image> Images { get; set; }

        public static MyServerDbContext Create()
        {
            return new MyServerDbContext();
        }

        public override int SaveChanges()
        {
            this.ApplyAuditInfoRules();

            foreach (var entry in this.ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted)) this.SoftDelete(entry);

            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Map(m => m.Requires("IsDeleted").HasValue(false)).Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Album>().Map(m => m.Requires("IsDeleted").HasValue(false)).Ignore(m => m.IsDeleted);
            modelBuilder.Entity<ImageGpsData>()
                .Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Image>().Map(m => m.Requires("IsDeleted").HasValue(false)).Ignore(m => m.IsDeleted);

            base.OnModelCreating(modelBuilder);
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

        private EntitySetBase GetEntitySet(Type type)
        {
            if (!_mappingCache.ContainsKey(type))
            {
                ObjectContext octx = ((IObjectContextAdapter)this).ObjectContext;

                string typeName = ObjectContext.GetObjectType(type).Name;

                var es =
                    octx.MetadataWorkspace.GetItemCollection(DataSpace.SSpace)
                        .GetItems<EntityContainer>()
                        .SelectMany(c => c.BaseEntitySets.Where(e => e.Name == typeName))
                        .FirstOrDefault();

                if (es == null) throw new ArgumentException("Entity type not found in GetTableName", typeName);

                _mappingCache.Add(type, es);
            }

            return _mappingCache[type];
        }

        private string GetPrimaryKeyName(Type type)
        {
            EntitySetBase es = this.GetEntitySet(type);

            return es.ElementType.KeyMembers[0].Name;
        }

        private string GetTableName(Type type)
        {
            EntitySetBase es = this.GetEntitySet(type);

            return string.Format(
                "[{0}].[{1}]", 
                es.MetadataProperties["Schema"].Value, 
                es.MetadataProperties["Table"].Value);
        }

        private void SoftDelete(DbEntityEntry entry)
        {
            Type entryEntityType = entry.Entity.GetType();

            string tableName = this.GetTableName(entryEntityType);
            string primaryKeyName = this.GetPrimaryKeyName(entryEntityType);

            string sql = string.Format("UPDATE {0} SET IsDeleted = 1 WHERE {1} = @id", tableName, primaryKeyName);

            this.Database.ExecuteSqlCommand(sql, new SqlParameter("@id", entry.OriginalValues[primaryKeyName]));

            // prevent hard delete            
            entry.State = EntityState.Detached;
        }
    }
}