using CampaignScheduler.Domain.Repository;
using CampaignScheduler.Domain.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CampaignScheduler.Domain.Helpers;

public class DomainDependencyRegistrar
{
    public static void RegisterDependencies(IServiceCollection services)
    {
        services.AddTransient(typeof(IRepository<>), typeof(MongoRepository<>));
        services.AddTransient<ICampaignRepository, CampaignMongoRepository>();
    }
}

