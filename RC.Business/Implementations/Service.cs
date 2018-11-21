using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RC.Business.Interfaces;
using RC.DataAccess.Interfaces;

namespace RC.Business.Implementations
{
    public class Service<TEntity> : IService<TEntity> where TEntity : class
    {
        #region ctor

        protected Service(IRepository<TEntity> repository)
        {
            _repository = repository;
        }

        #endregion ctor

        #region Private fields

        private readonly IRepository<TEntity> _repository;
        //private readonly ISystemLogger _systemLogger;

        #endregion

        protected DbContext Context
        {
            get { return _repository.Context; }
        }


        //protected ILogger Logger
        //{
        //    get { return LogManager.GetLogger(GetType().FullName); }
        //}

        #region GetById

        public virtual TEntity GetById(object id)
        {
            return _repository.GetObjectById(id);
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            return await _repository.GetObjectByIdAsync(id);
        }

        #endregion

        #region FirstOrDefault

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> criterias = null)
        {
            if (criterias != null)
            {
                return _repository.FirstOrDefault(criterias);
            }
            return _repository.FirstOrDefault();
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> criterias = null)
        {
            if (criterias != null)
            {
                return await _repository.FirstOrDefaultAsync(criterias);
            }
            return await _repository.FirstOrDefaultAsync();
        }

        #endregion

        #region Add

        public virtual int Add(TEntity entity)
        {
            return _repository.Add(entity);
        }

        public int AddRange(IEnumerable<TEntity> entities)
        {
            return _repository.AddRange(entities);
        }

        public async Task<int> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            return await _repository.AddRangeAsync(entities);
        }

        public virtual async Task<int> AddAsync(TEntity entity)
        {
            return await _repository.AddAsync(entity);
        }

        #endregion

        #region Update

        public virtual int Update(TEntity entity)
        {
            return _repository.Update(entity);
        }

        public virtual async Task<int> UpdateAsync(TEntity entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        #endregion

        #region Delete

        public virtual int Delete(object id)
        {
            return _repository.Delete(id);
        }

        public int Delete(Expression<Func<TEntity, bool>> criteria)
        {
            return _repository.Delete(criteria);
        }

        public int DeleteRange(IEnumerable<TEntity> entities)
        {
            return _repository.DeleteRange(entities);
        }

        public async Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            return await _repository.DeleteRangeAsync(entities);
        }

        public virtual async Task<int> DeleteAsync(TEntity entity)
        {
            return await _repository.DeleteAsync(entity);
        }

        public virtual async Task<int> DeleteAsync(object id)
        {
            return await _repository.DeleteAsync(id);
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> criteria)
        {
            await _repository.DeleteAsync(criteria);
        }

        #endregion
    }
}
