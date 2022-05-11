using EntityDock.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EntityDock
{
    /// <summary>
    /// This a abstraction to define a repository contract.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity, TID> where TEntity : AggregateRoot<TID>
    {
        /// <summary>
        /// This method return a queryable instance from current entity.
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> Get();

        /// <summary>
        /// Return all record  from current Entities.
        /// </summary>
        /// <returns></returns>
        Task<List<TEntity>> GetAll();
        
        /// <summary>
        /// Get a paginated list from current entity.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        Task<List<TEntity>> Get(int page, int length = 25);

        /// <summary>
        ///  Get a paginated and filtered list from current entity.
        /// </summary>
        /// <param name="Predicate"></param>
        /// <param name="page"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        Task<List<TEntity>> Get(Func<IQueryable<TEntity>, IQueryable<TEntity>> Predicate, int page = 0, int length = 25);

        /// <summary>
        /// Get one record by id from current entity.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntity> GetOne(TID id);
        
        /// <summary>
        /// Insert new record.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> StoreAnsyc(TEntity entity);
        
        /// <summary>
        /// Insert many new records. 
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        Task BulkStore(IEnumerable<TEntity> groups);

        /// <summary>
        /// Modify a new record.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(TEntity data);

        /// <summary>
        /// Modify a new record.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(TID id, TEntity data);

        /// <summary>
        /// Modify a new record.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(TEntity data, TEntity entity);

        /// <summary>
        /// Modify many new record.
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        Task BulkUpdate(IEnumerable<TEntity> groups);

        /// <summary>
        /// Fetch the relationship by property.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="id"></param>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public Task<IEnumerable<TProperty>> PullAsync<TProperty>(object id, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression) where TProperty : class;
        
        /// <summary>
        /// Add new element of target to push in the selected property.
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="id"></param>
        /// <param name="prop"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public Task<bool> PushAsync<TTarget>(object id, string prop, IEnumerable<TTarget> target);

        /// <summary>
        /// Remove a record from the current entity.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(TID id);

        /// <summary>
        /// Remove many records from the current entity.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(IEnumerable<TID> id);

        /// <summary>
        /// Remove a record from the current entity.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(TEntity data);

        /// <summary>
        /// Remove many records from the current entity.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(IEnumerable<TEntity> data);

        /// <summary>
        /// Remove many records by a query from the current entity.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<int> DeleteByQueryAsync(Action<IQueryable<TEntity>> query);
    }
}
