using CampaignScheduler.Config.Domain;
using CampaignScheduler.Domain.Models;
using CampaignScheduler.Domain.Repository.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CampaignScheduler.Domain.Repository;

public class MongoRepository<T> : IRepository<T> where T : IEntity
{
    protected readonly IMongoCollection<T> Collection;

    public MongoRepository(IOptions<MongoDbSettings> settings)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
        Collection = mongoDatabase.GetCollection<T>(ResolveCollectionName(typeof(T)));
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Collection.Find(_ => true).ToListAsync();
    }

    public IAsyncEnumerable<T> GetAllAsAsyncStream()
    {
        return Collection.Find(_ => true).ToAsyncEnumerable();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        var result = await Collection.Find(Builders<T>.Filter.Eq("_id", id)).FirstOrDefaultAsync();
        return result;
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter)
    {
        var result = await Collection.Find(filter).ToListAsync();
        return result;
    }

    public IAsyncEnumerable<T> FindAsAsyncStream(Expression<Func<T, bool>> filter)
    {
        return Collection.Find(filter).ToAsyncEnumerable();
    }

    public async Task AddAsync(T entity)
    {
        await Collection.InsertOneAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await Collection.InsertManyAsync(entities);
    }

    public async Task UpdateAsync(Guid id, T entity)
    {
        await Collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", id), entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await Collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
    }

    protected string ResolveCollectionName(MemberInfo entityType)
    {
        return entityType.Name.ToLower();
    }
}