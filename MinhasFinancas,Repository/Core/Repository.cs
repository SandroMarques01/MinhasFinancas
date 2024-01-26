using MinhasFinancas.Infra.Data;
using MinhasFinancas.Infra.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MinhasFinancas_Repository.Core
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {

        internal readonly AppDbContext _db;
        private readonly DbSet<TEntity> _dbSet;
        //private readonly string _connectionString = ConfigurationManager.ConnectionStrings["dbFinancasEntities"].ConnectionString;

        public Repository()
        {
            _db = new AppDbContext();
            _dbSet = _db.Set<TEntity>();

            _db.Configuration.ProxyCreationEnabled = false;
            _db.Configuration.LazyLoadingEnabled = false;

            //if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request.Url.Host.Contains("unifierhml.vinci-energies.com.br") && _db.Database.Connection.Database == "dbFinancasEntities")
            //    _db.Dispose();

            //if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request.Url.Host.Contains("unifier.vinci-energies.com.br") && _db.Database.Connection.Database == "dbFinancasEntities")
            //    _db.Dispose();

        }

        //public string GetConnectionString()
        //{
        //    return _connectionString;
        //}

        public virtual void Add(TEntity obj)
        {
            _dbSet.Add(obj);
        }
        public virtual void Update(TEntity obj)
        {
            _dbSet.Attach(obj);
            var oEntry = _db.Entry(obj);
            oEntry.State = EntityState.Modified;
        }


        public virtual void Delete(TEntity obj)
        {
            _dbSet.Attach(obj);
            _dbSet.Remove(obj);
        }

        public virtual void DeleteById(object id)
        {
            var obj = _dbSet.Find(id);

            if (obj != null)
                _dbSet.Remove(obj);
            else
                throw new KeyNotFoundException();
        }

        public virtual IEnumerable<TEntity> GetAll(
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         string includeProperties = null,
        int? skip = null,
         int? take = null)
        {
            return GetQueryable(null, orderBy, includeProperties, skip, take).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
        int? skip = null,
        int? take = null)
        {
            return await GetQueryable(null, orderBy, includeProperties, skip, take).ToListAsync();
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
        int? skip = null,
            int? take = null)
        {
            return GetQueryable(filter, orderBy, includeProperties, skip, take).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
        int? skip = null,
            int? take = null)
        {
            return await GetQueryable(filter, orderBy, includeProperties, skip, take).ToListAsync();
        }
        public virtual TEntity GetOne(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = "")
        {
            return GetQueryable(filter, null, includeProperties).SingleOrDefault();
        }

        public virtual async Task<TEntity> GetOneAsync(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null)
        {
            return await GetQueryable(filter, null, includeProperties).SingleOrDefaultAsync();
        }

        public virtual TEntity GetFirst(
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          string includeProperties = "")
        {
            return GetQueryable(filter, orderBy, includeProperties).FirstOrDefault();
        }


        public virtual async Task<TEntity> GetFirstAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null)
        {
            return await GetQueryable(filter, orderBy, includeProperties).FirstOrDefaultAsync();
        }

        public virtual TEntity GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual Task<TEntity> GetByIdAsync(object id)
        {
            return _dbSet.FindAsync(id);
        }

        public virtual int GetCount(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(filter).Count();
        }

        public virtual Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(filter).CountAsync();
        }

        public virtual bool GetExists(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(filter).Any();
        }

        public virtual Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(filter).AnyAsync();
        }
        protected virtual IQueryable<TEntity> GetQueryable(
                    Expression<Func<TEntity, bool>> filter = null,
                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                    string includeProperties = null,
                    int? skip = null,
                    int? take = null)
        {
            includeProperties = includeProperties ?? string.Empty;
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            //The method 'Skip' is only supported for sorted input in LINQ to Entities. 
            //The method 'OrderBy' must be called before the method 'Skip'.
            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query.AsNoTracking();
        }

        public int SaveChanges()
        {
            var saveChanges = 0;
            try
            {
                saveChanges = _db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var validationErros in e.EntityValidationErrors)
                {
                    foreach (var validationErro in validationErros.ValidationErrors)
                    {
                        System.ArgumentException ex = new System.ArgumentException(validationErro.ErrorMessage, validationErro.PropertyName, e);
                        throw ex;
                    }
                }
            }

            return saveChanges;
        }

        public DbContextTransaction BeginTransaction()
        {
            return _db.Database.BeginTransaction();
        }

        public DbContextTransaction BeginTransactionCanReadUncommitted()
        {
            return _db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
        }

        public string GetDatabaseName()
        {
            return _db.Database.Connection.Database;
        }
    }
}