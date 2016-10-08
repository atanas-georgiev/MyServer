namespace MyServer.Data
{
    using System;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    using MyServer.Data.Common.Models;
    using MyServer.Data.Models;

    public class MyServerDbContext : IdentityDbContext<User>
    {
        // private static readonly Dictionary<Type, EntitySetBase> MappingCache = new Dictionary<Type, EntitySetBase>();
        public MyServerDbContext(DbContextOptions<MyServerDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Album> Albums { get; set; }

        public virtual DbSet<Comment> Comments { get; set; }

        public virtual DbSet<ImageGpsData> ImageGpsDatas { get; set; }

        public virtual DbSet<Image> Images { get; set; }

        // public static MyServerDbContext Create()
        // {
        // return new MyServerDbContext();
        // }
        public override int SaveChanges()
        {
            this.ApplyAuditInfoRules();

            // foreach (var entry in this.ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted)) this.SoftDelete(entry);
            try
            {
                return base.SaveChanges();
            }
            catch (Exception ex)
            {
                return base.SaveChanges();
            }
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

            /*
            modelBuilder.Entity<User>().Map(m => m.Requires("IsDeleted").HasValue(false)).Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Album>().Map(m => m.Requires("IsDeleted").HasValue(false)).Ignore(m => m.IsDeleted);
            modelBuilder.Entity<ImageGpsData>()
                .Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Image>().Map(m => m.Requires("IsDeleted").HasValue(false)).Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Comment>().Map(m => m.Requires("IsDeleted").HasValue(false)).Ignore(m => m.IsDeleted);

            modelBuilder.Entity<Album>()
                .HasMany(x => x.Images)
                .WithOptional(x => x.Album)
                .HasForeignKey(x => x.AlbumId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Image>()
                .HasMany(x => x.Covers)
                .WithOptional(x => x.Cover)
                .HasForeignKey(x => x.CoverId)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Image>()
            //    .HasRequired(m => m.Album)
            //    .WithMany(t => t.Images)
            //    .HasForeignKey(t => t.AlbumId)
            //    .WillCascadeOnDelete(false);


            //modelBuilder.Entity<Image>()
            //    .HasOptional(x => x.Cover)
            //    .WithOptionalDependent(x => x.Cover)
            //    .WillCascadeOnDelete(false);
            */
            base.OnModelCreating(modelBuilder);
        }

        private void ApplyAuditInfoRules()
        {
            // Approach via @julielerman: http://bit.ly/123661P
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

        /*
        private EntitySetBase GetEntitySet(Type type)
        {
            if (!MappingCache.ContainsKey(type))
            {
                ObjectContext octx = ((IObjectContextAdapter)this).ObjectContext;

                string typeName = ObjectContext.GetObjectType(type).Name;

                var es =
                    octx.MetadataWorkspace.GetItemCollection(DataSpace.SSpace)
                        .GetItems<EntityContainer>()
                        .SelectMany(c => c.BaseEntitySets.Where(e => e.Name == typeName))
                        .FirstOrDefault();

                if (es == null) throw new ArgumentException("Entity type not found in GetTableName", typeName);

                MappingCache.Add(type, es);
            }

            return MappingCache[type];
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

        private void SoftDelete(EntityEntry entry)
        {
            Type entryEntityType = entry.Entity.GetType();

            string tableName = this.GetTableName(entryEntityType);
            string primaryKeyName = this.GetPrimaryKeyName(entryEntityType);

            string sql = string.Format("UPDATE {0} SET IsDeleted = 1 WHERE {1} = @id", tableName, primaryKeyName);

            this.Database.ExecuteSqlCommand(sql, new SqlParameter("@id", entry.OriginalValues[primaryKeyName]));

            // prevent hard delete            
            entry.State = EntityState.Detached;
        }*/
    }
}