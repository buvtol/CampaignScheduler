using CampaighScheduler.Core.Services.Interfaces;
using Quartz;

namespace CampaignScheduler.Jobs
{
    public class DailyUpdateJob : IJob
    {
        private readonly ILogger<DailyUpdateJob> _logger;
        private readonly ICampaignService _campaignService;

        public DailyUpdateJob(ILogger<DailyUpdateJob> logger, ICampaignService campaignService)
        {
            _logger = logger;
            _campaignService = campaignService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Daily update triggered {DateTime.Now.TimeOfDay}");

            await _campaignService.UpdateItemsToSchedule();
        }
    }
}
