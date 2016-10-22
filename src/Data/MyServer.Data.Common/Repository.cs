namespace MyServer.Data.Common
{
    using System;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    using MyServer.Data.Common.Models;

    public class Repository<T, TKey> : IRepository<T, TKey>
        where T : BaseModel<TKey>, IHavePrimaryKey<TKey> 
        where TKey : struct
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

        public virtual void Delete(TKey id)
        {
            var entity = this.GetById(id);

            if (entity != null)
            {
                this.Delete(entity);
            }

            this.Save();
        }

        public T GetById(TKey id)
        {
            return this.DbSet.FirstOrDefault(x => x.Id.Equals(id));
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