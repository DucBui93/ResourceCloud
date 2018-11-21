using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RC.DataAccess.Interfaces;

namespace RC.DataAccess.Implementations
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        internal readonly DbContext _dbContext;
        internal readonly DbSet<TEntity> _dbSet;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Database.EnsureCreated();
            _dbSet = dbContext.Set<TEntity>();
        }

        public DbContext Context
        {
            get { return _dbContext; }
        }

        #region FirstOrDefault

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return _dbSet.FirstOrDefault(predicate);
            }
            return _dbSet.FirstOrDefault();
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return await _dbSet.FirstOrDefaultAsync(predicate);
            }
            return await _dbSet.FirstOrDefaultAsync();
        }

        #endregion

        #region Add

        public virtual int Add(TEntity entity)
        {
            _dbSet.Add(entity);
            return _dbContext.SaveChanges();
        }

        public virtual int AddRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
            return _dbContext.SaveChanges();
        }

        public virtual async Task<int> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
            return await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<int> AddAsync(TEntity entity)
        {
            _dbSet.Add(entity);
            return await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region Update

        public virtual int Update(TEntity entity)
        {
            _dbSet.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            return _dbContext.SaveChanges();
        }

        public virtual async Task<int> UpdateAsync(TEntity entity)
        {
            try
            {
                _dbSet.Attach(entity);
                _dbContext.Entry(entity).State = EntityState.Modified;
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion

        #region Delete

        public virtual int Delete(object id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbContext.Entry(entity).State = EntityState.Deleted;
                _dbSet.Remove(entity);

                return _dbContext.SaveChanges();
            }

            return 0;
        }

        public virtual int DeleteRange(IEnumerable<TEntity> entities)
        {
            var enumerable = entities as TEntity[] ?? entities.ToArray();
            foreach (var entity in enumerable)
            {
                _dbContext.Entry(entity).State = EntityState.Deleted;
            }

            _dbSet.RemoveRange(enumerable);

            return _dbContext.SaveChanges();
        }

        public virtual async Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            var enumerable = entities as TEntity[] ?? entities.ToArray();
            foreach (var entity in enumerable)
            {
                _dbContext.Entry(entity).State = EntityState.Deleted;
            }

            _dbSet.RemoveRange(enumerable);

            return await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<int> DeleteAsync(object id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbContext.Entry(entity).State = EntityState.Deleted;
                _dbSet.Remove(entity);

                return await _dbContext.SaveChangesAsync();
            }
            return 0;
        }

        public virtual async Task<int> DeleteAsync(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Deleted;
            _dbSet.Remove(entity);

            return await _dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> criteria)
        {
            _dbSet.RemoveRange(_dbSet.Where(criteria));
            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region GetById

        public virtual TEntity GetObjectById(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual async Task<TEntity> GetObjectByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        #endregion

        /// <summary>
        /// Allows you to query an entity
        /// </summary>
        /// <param name="filter">Lambda expression for filtering rows</param>
        /// <param name="orderBy">Lambda expression for sorting</param>
        /// <param name="includedProperties">Add an argument for each property that should be eager loaded</param>
        /// <param name="page">When pageSize is greater then 0 then will return a particular data page</param>
        /// <param name="pageSize">Number of items per page. 0 will return all data without pages</param>
        /// <returns>An IEnumerable of the type or null if no data is found</returns>
        public virtual IEnumerable<TEntity> Query(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int page = 1,
            int pageSize = 0,
            params Expression<Func<TEntity, object>>[] includedProperties
        )
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includedProperties)
            {
                query.Include(includeProperty);
            }

            if (pageSize > 0)
            {
                query = query.Take(pageSize).Skip((page - 1) * pageSize);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

    }
}
