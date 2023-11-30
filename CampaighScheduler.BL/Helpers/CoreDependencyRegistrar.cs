using CampaighScheduler.Core.Services;
using CampaighScheduler.Core.Services.Interfaces;
using CampaignScheduler.Domain.Helpers;
using CampaignScheduler.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CampaighScheduler.Core.Helpers;

public class CoreDependencyRegistrar
{
    public static void RegisterDependencies(IServiceCollection services)
    {
        DomainDependencyRegistrar.RegisterDependencies(services);
        services.AddTransient<ICampaignService, CampaignService>();
        services.AddTransient<ICampaignSender, FileCampaignSender>();

        services.AddTransient<IChangeHandler<Customer>, CustomerChangeHandler>();
        services.AddTransient<IChangeHandler<Campaign>, CampaignChangeHandler>();
        services.AddTransient(typeof(ICollectionObserver<>), typeof(MongoCollectionObserver<>));
    }
}