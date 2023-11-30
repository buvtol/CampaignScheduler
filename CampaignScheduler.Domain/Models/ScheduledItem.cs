using CampaignScheduler.Domain.Enums;

namespace CampaignScheduler.Domain.Models;

public class ScheduledItem : IEntity
{
    public Guid Id { get; set; }
    public ScheduleItemStatus Status { get; set; }

    public TimeSpan SendTime { get; set; }

    public Guid CampaignId { get; set; }

    public Guid CustomerId { get; set; }
}