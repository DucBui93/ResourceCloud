using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RC.DataAccess.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        #region Add

        int Add(TEntity entity);

        Task<int> AddAsync(TEntity entity);

        int AddRange(IEnumerable<TEntity> entities);

        Task<int> AddRangeAsync(IEnumerable<TEntity> entities);

        #endregion

        #region Update

        int Update(TEntity entity);

        Task<int> UpdateAsync(TEntity entity);

        #endregion

        #region Delete

        int Delete(object id);

        int DeleteRange(IEnumerable<TEntity> entities);

        Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities);

        Task<int> DeleteAsync(object id);

        Task<int> DeleteAsync(TEntity entity);

        Task DeleteAsync(Expression<Func<TEntity, bool>> criteria);

        #endregion

        #region GetById

        TEntity GetObjectById(object id);

        Task<TEntity> GetObjectByIdAsync(object id);

        #endregion

        #region FirstOrDefault

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate = null);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null);

        #endregion
        IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int page = 1, int pageSize = 0, params Expression<Func<TEntity, object>>[] includedProperties);

        DbContext Context { get; }
    }
}
