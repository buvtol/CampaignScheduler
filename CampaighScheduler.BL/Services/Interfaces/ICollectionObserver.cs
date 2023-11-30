namespace CampaighScheduler.Core.Services.Interfaces;

public interface ICollectionObserver<T>
{
    Task StartObservingAsync(CancellationToken cancellationToken = default);

    void StopObserving();
}