using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace CampaignScheduler.Domain.Models;

public class Campaign : IEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string SerializedCondition { get; set; } = null!;

    public TimeSpan SendTime { get; set; }

    public int Priority { get; set; }
}