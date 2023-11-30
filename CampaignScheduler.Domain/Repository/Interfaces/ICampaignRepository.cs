using CampaignScheduler.Domain.Models;

namespace CampaignScheduler.Domain.Repository.Interfaces;

public interface ICampaignRepository : IRepository<Campaign>
{
    Task<IEnumerable<Campaign>> GetAllOrderedCampaignsAsync();
    IAsyncEnumerable<Campaign> GetAllOrderedCampaignsAsyncEnumerable();
}