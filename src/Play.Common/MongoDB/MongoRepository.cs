using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Play.Common;

namespace Play.Common.MongoDB
{

    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> _dbCollection;
        private readonly FilterDefinitionBuilder<T> _filterBuilder = Builders<T>.Filter;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            _dbCollection = database.GetCollection<T>(collectionName);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await _dbCollection.Find(_filterBuilder.Empty).ToListAsync();
        }

        public async Task<T> GetItemAsync(Guid id)
        {
            FilterDefinition<T> filter = _filterBuilder.Eq(entity => entity.Id, id);
            return await _dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _dbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            FilterDefinition<T> filter = _filterBuilder.Eq(entityExisting => entityExisting.Id, entity.Id);

            await _dbCollection.ReplaceOneAsync(filter, entity);
        }

        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<T> filter = _filterBuilder.Eq(entity => entity.Id, id);
            await _dbCollection.DeleteOneAsync(filter);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbCollection.Find(filter).ToListAsync();
        }

        public async Task<T> GetItemAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}