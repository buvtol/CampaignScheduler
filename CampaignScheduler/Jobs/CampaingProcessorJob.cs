using CampaighScheduler.Core.Services.Interfaces;
using Quartz;

namespace CampaignScheduler.Jobs;

public class CampaingProcessorJob : IJob
{
    private readonly ILogger<CampaingProcessorJob> _logger;
    private readonly ICampaignService _campaignSelector;
    private readonly ICampaignSender _campaignSender;

    public CampaingProcessorJob(ILogger<CampaingProcessorJob> logger, ICampaignService campaignSelector, ICampaignSender campaignSender)
    {
        _logger = logger;
        _campaignSelector = campaignSelector;
        _campaignSender = campaignSender;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"Campaign processing triggered {DateTime.Now.TimeOfDay}");

        var scheduledItems = _campaignSelector.GetScheduledItemsByTimeAsyncEnumerable(DateTime.Now.TimeOfDay);

        //todo limit concurrency
        var sendTasks = new List<Task>();

        await foreach (var schedule in scheduledItems)
        {
            var task = Task.Run(() => _campaignSender.SendCampaign(schedule));
            sendTasks.Add(task);
        }

        await Task.WhenAll(sendTasks);

        _logger.LogInformation($"Campaign processed in total {sendTasks.Count}");
    }
}