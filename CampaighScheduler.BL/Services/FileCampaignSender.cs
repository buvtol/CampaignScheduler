using CampaighScheduler.Core.Services.Interfaces;
using CampaignScheduler.Config.Core;
using CampaignScheduler.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CampaighScheduler.Core.Services
{
    public class FileCampaignSender : ICampaignSender
    {
        private readonly ILogger _logger;
        private readonly IOptions<FileSenderSettings> _configuration;

        public FileCampaignSender(ILogger<FileCampaignSender> logger, IOptions<FileSenderSettings> configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        private readonly SemaphoreSlim _fileWriteSemaphore = new(1, 1);

        public async Task SendCampaign(ScheduledItem scheduledItem)
        {
            try
            {
                await _fileWriteSemaphore.WaitAsync();

                var content = JsonConvert.SerializeObject(scheduledItem);

                var fileName = string.Format(_configuration.Value.FileNameTemplate, DateTime.UtcNow.ToString(_configuration.Value.DateFormat));

                await File.AppendAllTextAsync(fileName, content + Environment.NewLine, Encoding.UTF8);
            }
            catch (Exception)
            {
                _logger.LogError($"File writing error {DateTime.UtcNow}");
            }
            finally
            {
                _fileWriteSemaphore.Release();
            }
        }
    }
}

