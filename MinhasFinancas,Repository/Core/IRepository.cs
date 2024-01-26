using MinhasFinancas.Infra.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace MinhasFinancas_Repository.Core
{
    public interface IRepository<TEntity> where TEntity : Entity
    {

        void Add(TEntity obj);
        void Update(TEntity obj);
        void Delete(TEntity obj);
        void DeleteById(object id);
        DbContextTransaction BeginTransaction();
        int SaveChanges();

        IEnumerable<TEntity> GetAll(
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string includeProperties = null,
           int? skip = null,
           int? take = null);

        Task<IEnumerable<TEntity>> GetAllAsync(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null);

        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null);

        Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null);

        TEntity GetOne(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null);

        Task<TEntity> GetOneAsync(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null);

        TEntity GetFirst(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null);

        Task<TEntity> GetFirstAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null);

        TEntity GetById(object id);

        Task<TEntity> GetByIdAsync(object id);

        /// <summary>
        /// Return number 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        int GetCount(Expression<Func<TEntity, bool>> filter = null);

        Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter = null);

        bool GetExists(Expression<Func<TEntity, bool>> filter = null);

        Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> filter = null);

        string GetDatabaseName();

        DbContextTransaction BeginTransactionCanReadUncommitted();
    }
}
