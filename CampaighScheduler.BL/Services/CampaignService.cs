using CampaighScheduler.Core.Helpers;
using CampaighScheduler.Core.Services.Interfaces;
using CampaignScheduler.Domain.Enums;
using CampaignScheduler.Domain.Models;
using CampaignScheduler.Domain.Repository.Interfaces;

namespace CampaighScheduler.Core.Services
{
    public class CampaignService: ICampaignService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IRepository<ScheduledItem> _scheduleRepository;

        public CampaignService(
            IRepository<Customer> customerRepository,
            ICampaignRepository campaignRepository,
            IRepository<ScheduledItem> scheduleRepository)
        {
            _customerRepository = customerRepository;
            _campaignRepository = campaignRepository;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<IEnumerable<ScheduledItem>> GetPendingItems() =>
            await _scheduleRepository.FindAsync(x => x.Status == ScheduleItemStatus.Pending);

        public IAsyncEnumerable<ScheduledItem> GetScheduledItemsByTimeAsyncEnumerable(TimeSpan time) =>
            _scheduleRepository.FindAsAsyncStream(x => x.SendTime == time);

        public async Task UpdateItemsToSchedule() =>
            await _scheduleRepository.AddRangeAsync(await GetItemsToSchedule());

        public async Task ScheduleCustomer(Guid customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);

            await foreach (var campaign in _campaignRepository.GetAllOrderedCampaignsAsyncEnumerable())
            {
                if (CampaignConditionHelper.CheckCondition(campaign.SerializedCondition, customer))
                {
                    await _scheduleRepository.AddAsync(CreatePendingScheduledItem(campaign.Id, customer.Id, campaign.SendTime));
                    break;
                }
            }
        }

        //todo implement
        public Task ScheduleCampaign(Guid campaignId) => throw new NotImplementedException();

        public Task RemovePendingSchedules() => throw new NotImplementedException();

        private async Task<IEnumerable<ScheduledItem>> GetItemsToSchedule()
        {
            var schedule = new List<ScheduledItem>();

            var campaigns = await _campaignRepository.GetAllOrderedCampaignsAsync();

            await foreach (var customer in _customerRepository.GetAllAsAsyncStream())
            {
                foreach (var campaign in campaigns)
                {
                    if (CampaignConditionHelper.CheckCondition(campaign.SerializedCondition, customer))
                    {
                        schedule.Add(CreatePendingScheduledItem(campaign.Id, customer.Id, campaign.SendTime));
                        break;
                    }
                }
            }

            await _scheduleRepository.AddRangeAsync(schedule);

            return schedule;
        }

        private ScheduledItem CreatePendingScheduledItem(Guid campaignId, Guid customerId, TimeSpan sendTime) =>
            new()
            {
                Id = Guid.NewGuid(),
                CampaignId = campaignId,
                CustomerId = customerId,
                Status = ScheduleItemStatus.Pending,
                SendTime = sendTime
            };
    }
}

