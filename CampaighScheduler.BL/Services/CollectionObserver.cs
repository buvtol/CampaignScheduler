using CampaighScheduler.Core.Services.Interfaces;
using CampaignScheduler.Config.Domain;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Reflection;

namespace CampaighScheduler.Core.Services;

public class MongoCollectionObserver<T> : ICollectionObserver<T>
{
    private readonly IMongoCollection<T> _collection;
    private readonly IChangeHandler<T> _changeHandler;
    private IChangeStreamCursor<ChangeStreamDocument<T>> _cursor = null!;

    public MongoCollectionObserver(IOptions<MongoDbSettings> settings, IChangeHandler<T> changeHandler)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _collection = mongoDatabase.GetCollection<T>(ResolveCollectionName(typeof(T)));

        _changeHandler = changeHandler;
    }

    public async Task StartObservingAsync(CancellationToken cancellationToken = default)
    {
        var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup };

        _cursor = await _collection.WatchAsync(options, cancellationToken);

        await foreach (var change in _cursor.ToAsyncEnumerable(cancellationToken))
        {
            await _changeHandler.HandleChange(change);
        }
    }

    public void StopObserving()
    {
        _cursor?.Dispose();
    }

    //todo resolve via config
    protected string ResolveCollectionName(MemberInfo entityType)
    {
        return entityType.Name.ToLower();
    }
}