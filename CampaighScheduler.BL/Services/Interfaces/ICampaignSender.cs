using System;
using CampaignScheduler.Domain.Models;

namespace CampaighScheduler.Core.Services.Interfaces
{
    public interface ICampaignSender
    {
        Task SendCampaign(ScheduledItem scheduledItem);
    }
}

