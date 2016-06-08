namespace MyServer.Data.Common
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    using MyServer.Data.Common.Models;

    public interface IRepository<T> : IRepository<T, string>
        where T : class, IHavePrimaryKey<string>, IDeletableEntity, IAuditInfo
    {
    }

    public interface IRepository<T, in TKey> : IDbGenericRepository<T, TKey>
        where T : class, IHavePrimaryKey<TKey>, IDeletableEntity, IAuditInfo
    {
        IQueryable<T> AllWithDeleted();

        void HardDelete(T entity);
    }

    public interface IDbGenericRepository<T, in TKey>
        where T : class
    {
        void Add(T entity);

        IQueryable<T> All();

        void Delete(T entity);

        void Delete(TKey entity);

        T GetById(TKey id);

        void Save();

        void Update(T entity);
    }

    public interface IHavePrimaryKey<TKey>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        TKey Id { get; set; }
    }
}