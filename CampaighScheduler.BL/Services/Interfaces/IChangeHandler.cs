using MongoDB.Driver;

namespace CampaighScheduler.Core.Services.Interfaces;

public interface IChangeHandler<T>
{
    Task HandleChange(ChangeStreamDocument<T> change);
}