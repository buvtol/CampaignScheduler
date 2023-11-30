using CampaighScheduler.Core.Services.Interfaces;
using CampaignScheduler.Domain.Models;
using MongoDB.Driver;

namespace CampaighScheduler.Core.Services;

public class CampaignChangeHandler : IChangeHandler<Campaign>
{
    private readonly ICampaignService _campaignService;

    public CampaignChangeHandler(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }
    public async Task HandleChange(ChangeStreamDocument<Campaign> change)
    {
        //await _campaignService.ScheduleCampaign(change.FullDocument.Id);
        await _campaignService.UpdateItemsToSchedule();
    }
}