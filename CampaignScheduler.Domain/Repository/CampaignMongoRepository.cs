using CampaignScheduler.Config.Domain;
using CampaignScheduler.Domain.Models;
using CampaignScheduler.Domain.Repository.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace CampaignScheduler.Domain.Repository;

public class CampaignMongoRepository: MongoRepository<Campaign>, ICampaignRepository
{
    public CampaignMongoRepository(IOptions<MongoDbSettings> settings) : base(settings)
    {
    }

    public async Task<IEnumerable<Campaign>> GetAllOrderedCampaignsAsync()
    {
        return await Collection.Find(_ => true)
            .SortBy(campaign => campaign.Priority)
            .ToListAsync();
    }

    public IAsyncEnumerable<Campaign> GetAllOrderedCampaignsAsyncEnumerable()
    {
        return Collection.Find(_ => true)
            .SortBy(campaign => campaign.Priority)
            .ToAsyncEnumerable();
    }
}