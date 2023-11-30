using CampaignScheduler.Domain.Models;

namespace CampaighScheduler.Core.Services.Interfaces
{
    public interface ICampaignService
    {
        Task<IEnumerable<ScheduledItem>> GetPendingItems();

        IAsyncEnumerable<ScheduledItem> GetScheduledItemsByTimeAsyncEnumerable(TimeSpan time);

        Task UpdateItemsToSchedule();

        Task ScheduleCustomer(Guid customerId);

        Task ScheduleCampaign(Guid campaignId);

        Task RemovePendingSchedules();
    }
}