using AutoMapper;
using EntityDock.Entities.Base;
using EntityDock.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EntityDock.Persistence
{
    /// <summary>
    /// Data Service allow interact with entities repository.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TGet"></typeparam>
    /// <typeparam name="TAdd"></typeparam>
    /// <typeparam name="TID"></typeparam>
    public class DataService<TEntity, TGet, TAdd, TID>
    where TEntity : AggregateRoot<TID>
    where TGet : class
    where TAdd : class
    {
        /// <summary>
        /// Require a base service.
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="repository"></param>
        public DataService(IMapper mapper, IRepository<TEntity, TID> repository)
        {
            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            Mapper = mapper;
            Repository = repository;
        }

        /// <summary>
        /// The mapper service.
        /// </summary>
        public IMapper Mapper { get; }

        /// <summary>
        /// The target repository.
        /// </summary>
        public IRepository<TEntity, TID> Repository { get; }

        /// <summary>
        /// Indicate if has a mapper schema for get data. 
        /// </summary>
        public bool HasMapGet { get; internal set; }

        /// <summary>
        /// Indicate if has a mapper schema for add data. 
        /// </summary>
        public bool HasMapAdd{ get; internal set; }

    /// <summary>
    /// Get all record without filter or other options.
    /// </summary>
    /// <returns></returns>
    public async Task<List<TGet>> AllAsync()
        {
            var result = await Repository.Get().ToArrayAsync();

            // check if has map
            if (HasMapGet)
            {
                return result.ToList() as List<TGet>;
            }
            else
            {
                return result.Select(x => Mapper.Map<TGet>(x)).ToList();
            }
        }

        /// <summary>
        /// Fetch a record by id is a method to find specific data.
        /// </summary>
        /// <typeparam name="Tid"></typeparam>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<TGet> FindAsync(TID id)
        {
            return Mapper.Map<TGet>(await Repository.GetOne(id));
        }

        /// <summary>
        /// Create new record from a entry object.
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<InsertionResult<TGet>> InsertRecord(TAdd post)
        {
            var result = new InsertionResult<TGet>();

            try
            {
                var data = HasMapAdd ? Mapper.Map<TAdd, TEntity>(post) : post as TEntity;

                // set write record count
                result.WritesRecords = await Repository.StoreAnsyc(data);

                // set result 
                result.Result = HasMapGet ? Mapper.Map<TGet>(post) : data as TGet;
            }
            catch (Exception err)
            {
                result.Error = err;
            }

            return result;
        }

        /// <summary>
        /// Add many entities as batch.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task InsertBatch(IEnumerable<TAdd> data)
        {
            // check if map is enabled
            if (HasMapAdd)
                return Repository.BulkStore(data.UseMapper<TAdd, TEntity>(Mapper));
            else
                return Repository.BulkStore(data as IEnumerable<TEntity>);
        }

        /// <summary>
        /// Delete a specific record.
        /// </summary>
        /// <typeparam name="Tid"></typeparam>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync(TID id)
        {
            int writes = await Repository.DeleteAsync(id);
            return writes > 0;
        }

        /// <summary>
        /// Update a record by any object passed.
        /// If entity is different then this emthod use the mapper
        /// to resolve entity data.
        /// </summary>
        /// <param name="postObj"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(object postObj)
        {
            Type sourceType = postObj.GetType();
            bool notMapp = sourceType.IsEquivalentTo(typeof(TEntity));

            // check if exist mapper for postObj and the current entity
            if (Mapper.ConfigurationProvider.FindMapper(new TypePair(sourceType, typeof(TEntity))) is null)
            {
                return false;
            }

            int writes = await Repository.UpdateAsync(notMapp ? (TEntity) postObj : Mapper.Map<TEntity>(postObj));
            return writes > 0;
        }

        /// <summary>
        /// Update a batch by any object passed.
        /// </summary>
        /// <param name="postObj"></param>
        /// <returns></returns>
        public async Task<bool> UpdateBatchAsync(List<object> postObj)
        {
            if (postObj.Count < 1)
            {
                return false;
            }

            Type sourceType = postObj[0].GetType();
            bool notMapp = sourceType.IsEquivalentTo(typeof(TEntity));

            // check if exist mapper for postObj and the current entity
            // check mapper configuration
            if (notMapp is false && Mapper.ConfigurationProvider.FindMapper(new TypePair(sourceType, typeof(TEntity))) is null)
            {
                return false;
            }

            // check if mapper is required
            if (notMapp)
                await Repository.BulkUpdate(postObj.Cast<TEntity>());
            else
                await Repository.BulkUpdate(postObj.UseMapper<object, TEntity>(Mapper));

            return true;
        }

        /// <summary>
        /// Update a record by any object passed
        /// </summary>
        /// <param name="postObj"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(TID idValue, object postObj)
        {
            // check if exist mapper for postObj and the current entity
            if (Mapper.ConfigurationProvider.FindMapper(new TypePair(postObj.GetType(), typeof(TEntity))) is null)
            {
                return false;
            }

            var data = Mapper.Map<TEntity>(postObj);
            int writes = await Repository.UpdateAsync(idValue, data);
            return writes > 0;
        }
    }

    /// <summary>
    /// Implemetation by a one entity without mappers cases
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TID"></typeparam>
    public class DataService<TEntity, TID> : DataService<TEntity, TEntity, TEntity, TID>
    where TEntity : AggregateRoot<TID>
    {
        public DataService(IMapper mapper, IRepository<TEntity, TID> repository) : base(mapper, repository)
        {
            HasMapGet = false;
            HasMapAdd = false;
        }
    }

    /// <summary>
    /// Implemetation by a one entity with only posts classes
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPost"></typeparam>
    /// <typeparam name="TID"></typeparam>
    public class DataService<TEntity, TPost, TID> : DataService<TEntity, TEntity, TPost, TID>
    where TEntity : AggregateRoot<TID>
    where TPost : class
    {
        public DataService(IMapper mapper, IRepository<TEntity, TID> repository) : base(mapper, repository)
        {
            HasMapGet = false;
            HasMapAdd = true;
        }
    }
}