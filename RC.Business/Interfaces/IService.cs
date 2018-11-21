using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RC.Business.Interfaces
{
    public interface IService<TEntity> where TEntity : class
    {
        #region Add

        int Add(TEntity entity);

        int AddRange(IEnumerable<TEntity> entities);

        Task<int> AddRangeAsync(IEnumerable<TEntity> entities);

        Task<int> AddAsync(TEntity entity);

        #endregion

        #region Update

        int Update(TEntity entity);

        Task<int> UpdateAsync(TEntity entity);

        #endregion

        #region Delete

        int Delete(object id);

        int Delete(Expression<Func<TEntity, bool>> criteria);

        int DeleteRange(IEnumerable<TEntity> entities);

        Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities);

        Task<int> DeleteAsync(object id);

        Task<int> DeleteAsync(TEntity entity);

        Task DeleteAsync(Expression<Func<TEntity, bool>> criteria);

        #endregion

        #region GetById

        TEntity GetById(object id);

        Task<TEntity> GetByIdAsync(object id);

        #endregion

        #region FirstOrDefault 

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> criterias = null);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> criterias = null);

        #endregion
    }
}
