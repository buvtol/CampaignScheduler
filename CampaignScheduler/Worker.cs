using CampaighScheduler.Core.Services.Interfaces;
using CampaignScheduler.Domain.Models;
using CampaignScheduler.Jobs;
using Quartz;

namespace CampaignScheduler;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly ICollectionObserver<Campaign> _campaignObserver;
    private readonly ICollectionObserver<Customer> _customerObserver;
    private IScheduler _scheduler;

    public Worker(ILogger<Worker> logger, ISchedulerFactory schedulerFactory,
        ICollectionObserver<Campaign> campaignObserver, ICollectionObserver<Customer> customerObserver)
    {
        _logger = logger;
        _schedulerFactory = schedulerFactory;
        _campaignObserver = campaignObserver;
        _customerObserver = customerObserver;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker started execution");

        await ScheduleJobs();

        while (!stoppingToken.IsCancellationRequested)
        {
            var observeTasks = new[]
            {
                _campaignObserver.StartObservingAsync(stoppingToken),
                _customerObserver.StartObservingAsync(stoppingToken)
            };

            await Task.WhenAll(observeTasks);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _campaignObserver.StopObserving();
        _customerObserver.StopObserving();

        await _scheduler?.Shutdown(cancellationToken)!;

        await base.StopAsync(cancellationToken);
    }

    protected async Task ScheduleJobs()
    {
        _scheduler = await _schedulerFactory.GetScheduler();

        await _scheduler.Start();

        var dailyUpdate = JobBuilder.Create<DailyUpdateJob>()
            .WithIdentity(nameof(DailyUpdateJob))
            .Build();

        var dailyUpdateTringger = TriggerBuilder.Create()
            .WithIdentity(nameof(DailyUpdateJob))
            .StartNow()
            .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(0, 0))
            .Build();

        var campaignProcessor = JobBuilder.Create<CampaingProcessorJob>()
            .WithIdentity(nameof(CampaingProcessorJob))
            .Build();

        var campaignProcessorTrigger = TriggerBuilder.Create()
            .WithIdentity(nameof(CampaingProcessorJob))
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(1)
                .RepeatForever())
            .Build();

        await _scheduler.ScheduleJob(campaignProcessor, campaignProcessorTrigger);
        await _scheduler.ScheduleJob(dailyUpdate, dailyUpdateTringger);
    }
}
