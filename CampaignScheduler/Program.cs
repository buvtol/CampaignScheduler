using CampaighScheduler.Core.Helpers;
using CampaignScheduler.Config.Core;
using CampaignScheduler.Config.Domain;
using CampaignScheduler.Jobs;
using Quartz;
using Quartz.Impl;

namespace CampaignScheduler;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        host.RunAsync();

        await Task.Delay(TimeSpan.FromMinutes(30));

        await host.StopAsync();
    }
    
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                ConfigureServices(services);
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                });
                services.AddHostedService<Worker>();
            });

    private static void ConfigureServices(IServiceCollection services)
    {
        var configuration = BuildConfiguration();

        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
        services.AddTransient<CampaingProcessorJob>();
        services.AddTransient<DailyUpdateJob>();

        services.Configure<FileSenderSettings>(configuration.GetSection(nameof(FileSenderSettings)));
        services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings))); 

        CoreDependencyRegistrar.RegisterDependencies(services);
    }

    private static IConfiguration BuildConfiguration() =>
        new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
}