using CampaighScheduler.Core.Services.Interfaces;
using CampaignScheduler.Domain.Models;
using MongoDB.Driver;

namespace CampaighScheduler.Core.Services;

public class CustomerChangeHandler : IChangeHandler<Customer>
{
    private readonly ICampaignService _campaignService;

    public CustomerChangeHandler(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }
    public async Task HandleChange(ChangeStreamDocument<Customer> change)
    {
        await _campaignService.ScheduleCustomer(change.FullDocument.Id);
    }
}