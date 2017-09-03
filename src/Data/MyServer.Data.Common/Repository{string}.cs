namespace MyServer.Data.Common
{
    using System;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    using MyServer.Data.Common.Models;

    public class Repository<T> : IRepository<T>
        where T : class, IHavePrimaryKey<string>, IDeletableEntity, IAuditInfo
    {
        public Repository(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentException(
                    "An instance of DbContext is required to use this repository.",
                    nameof(context));
            }

            this.Context = context;
            this.DbSet = this.Context.Set<T>();
        }

        private DbContext Context { get; }

        private DbSet<T> DbSet { get; }

        public void Add(T entity)
        {
            this.DbSet.Add(entity);
            this.Save();
        }

        public DbSet<T> All()
        {
            return this.DbSet;
        }

        public IQueryable<T> AllWithDeleted()
        {
            return this.DbSet;
        }

        public void Delete(T entity)
        {
            entity.DeletedOn = DateTime.Now;
            entity.IsDeleted = true;
            this.Save();
            this.DbSet.Remove(entity);
            this.Save();
        }

        public virtual void Delete(string id)
        {
            var entity = this.GetById(id);

            if (entity != null)
            {
                this.Delete(entity);
            }

            this.Save();
        }

        public T GetById(string id)
        {
            return this.All().FirstOrDefault(x => x.Id == id);
        }

        public void HardDelete(T entity)
        {
            this.DbSet.Remove(entity);
            this.Save();
        }

        public void Save()
        {
            this.Context.SaveChanges();
        }

        public void Update(T entity)
        {
            EntityEntry entry = this.Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.DbSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
            this.Save();
        }
    }
}