using EntityDock.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EntityDock.Persistence
{
    /// <summary>
    /// The repository <see cref="IRepository{TEntity, TID}"/>
    /// implementation for all type entities
    /// </summary>
    /// <typeparam name="TargetEntity"></typeparam>
    /// <typeparam name="TID"></typeparam>
    public class RepositoryFactory<TargetEntity, TID> : IRepository<TargetEntity, TID>
        // require root rule
        where TargetEntity : AggregateRoot<TID>
    {
        /// <summary>
        /// Required a system context for <see cref="DbContext"/>
        /// </summary>
        /// <param name="context"></param>
        public RepositoryFactory(DbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            TypeEntity = typeof(TargetEntity);
        }

        /// <summary>
        /// Application context reference
        /// </summary>
        DbContext Context { get; }

        /// <summary>
        /// Type entity reference
        /// </summary>
        Type TypeEntity { get; }

        /// <summary>
        /// Bulk operations for inserts
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        public Task BulkStore(IEnumerable<TargetEntity> groups)
        {
            return Context.BulkInsertAsync(groups, options =>
            {
                options.BatchSize = sbyte.MaxValue;
                options.ErrorMode = Z.BulkOperations.ErrorModeType.RetrySingleAndContinue;
            });
        }

        /// <summary>
        /// Bulk operation for updates
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        public Task BulkUpdate(IEnumerable<TargetEntity> groups)
        {
            return Context.BulkUpdateAsync(groups, options =>
            {
                options.BatchSize = sbyte.MaxValue;
                options.ErrorMode = Z.BulkOperations.ErrorModeType.RetrySingleAndContinue;
            });
        }

        /// <summary>
        /// The basic implemntation for delete by query
        /// </summary>
        /// <typeparam name="TId"></typeparam>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Task<int> DeleteByQueryAsync(Action<IQueryable<TargetEntity>> action)
        {
            var query = Get();
            action.Invoke(query);
            return query.OfType<TargetEntity>().DeleteFromQueryAsync();
        }

        /// <summary>
        /// The basic implemntation for delete a element by id value
        /// </summary>
        /// <typeparam name="TId"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(TID id)
        {
            Context.Remove(await Context.FindAsync(TypeEntity, id).AsTask());
            return await Context.SaveChangesAsync(default);
        }

        /// <summary>
        /// The basic implemntation for delete a element by entity isntance
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync(TargetEntity data)
        {
            Context.Remove(data);
            return Context.SaveChangesAsync(default);
        }

        /// <summary>
        /// The basic implemntation for delete a element by id value in for collection
        /// </summary>
        /// <typeparam name="TId"></typeparam>
        /// <param name="elements"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(IEnumerable<TID> elements)
        {
            foreach (var id in elements)
            {
                Context.Remove(await Context.FindAsync(TypeEntity, id).AsTask());
            }

            // return a entries numbers
            return await Context.SaveChangesAsync(default);
        }

        /// <summary>
        /// The basic implemntation for delete a element by entity isntance for collection
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync(IEnumerable<TargetEntity> data)
        {
            foreach (var item in data)
            {
                Context.Remove(item);
            }

            // return a entries numbers
            return Context.SaveChangesAsync(default);
        }

        /// <summary>
        /// Get the DbSet instance as IQueryable<TargetEntity>
        /// </summary>
        /// <returns></returns>
        public IQueryable<TargetEntity> Get()
        {
            return Context.Set<TargetEntity>();
        }

        /// <summary>
        /// Get paginated list
        /// </summary>
        /// <param name="page"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public Task<List<TargetEntity>> Get(int page, int length = 25)
        {
            return Context.Set<TargetEntity>()
                .AsNoTracking()
                .Skip((page - 1) * length)
                .Take(length)
                .ToListAsync();
        }

        /// <summary>
        /// Get filter and paginate list list 
        /// </summary>
        /// <param name="Predicate"></param>
        /// <param name="page"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public Task<List<TargetEntity>> Get(Func<IQueryable<TargetEntity>, IQueryable<TargetEntity>> Predicate, int page = 0, int length = 25)
        {
            var collection = Context
                .Set<TargetEntity>()
                .AsNoTracking();

            // the page applied
            if (page > 0)
            {
                collection.Skip((page - 1) * length).Take(length);
            }

            // apply predicate
            return Predicate(collection).ToListAsync();
        }

        /// <summary>
        /// Get all records
        /// </summary>
        /// <returns></returns>
        public Task<List<TargetEntity>> GetAll()
        {
            // with no tracking level
            return Context.Set<TargetEntity>()
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Get a single record by id
        /// </summary>
        /// <typeparam name="TId"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TargetEntity> GetOne(TID id)
        {
            var result = await Context.FindAsync(TypeEntity, id).AsTask();
            if (result is TargetEntity data)
            {
                return data;
            }
            throw new InvalidCastException();
        }

        /// <summary>
        /// Get relation
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="id"></param>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TTarget>> PullAsync<TTarget>(object id, [NotNull] Expression<Func<TargetEntity, IEnumerable<TTarget>>> propertyExpression)
        where TTarget : class
        {
            var data = await Context.FindAsync<TargetEntity>(id);

            if (data is null)
                return null;

            await Context.Entry(data)
                .Collection(propertyExpression)
                .LoadAsync();

            return propertyExpression.Compile().Invoke(data);
        }

        /// <summary>
        /// Register new records
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="id"></param>
        /// <param name="prop"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public Task<bool> PushAsync<TTarget>(object id, string prop, TTarget target)
        {
            return PushAsync(id, prop, new List<TTarget> { target });
        }

        /// <summary>
        /// Register relation data
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="id"></param>
        /// <param name="prop"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public async Task<bool> PushAsync<TTarget>(object id, string prop, IEnumerable<TTarget> target)
        {
            var data = await Context.FindAsync<TargetEntity>(id);

            if (data is null)
                return false;

            // define an entry
            Context.Entry(data).Property(prop).CurrentValue = target.ToList();

            // return a entries numbers
            var entries = await Context.SaveChangesAsync();

            return entries > 0;
        }

        /// <summary>
        /// Add new record by instance
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> StoreAnsyc(TargetEntity entity)
        {
            Context.Add(entity);
            return Context.SaveChangesAsync(default);
        }

        /// <summary>
        ///  Update new record by instance
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(TargetEntity data)
        {
            Context.Update(data);
            return Context.SaveChangesAsync(default);
        }

        /// <summary>
        ///  Update new record by instance and check concurrency comprobation
        /// </summary>
        /// <param name="data"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(TargetEntity data, TargetEntity entity)
        {
            Context.Update(data);
            return Context.SaveChangesAsync(default);
        }

        /// <summary>
        /// If the Id is not set in PAYLOAD passed then you can pass specific record id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(TID id, TargetEntity data)
        {
            data.Id = id;
            Context.Update(data);
            return Context.SaveChangesAsync(default);
        }
    }
}
