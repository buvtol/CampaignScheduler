using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace CampaignScheduler.Domain.Models;

public class Customer: IEntity
{
    public Guid Id { get; set; }

    public int? Age { get; set; }
    public Gender? Gender { get; set; }
    public string? City { get; set; }
    public decimal DepositAmount { get; set; }
    public bool IsNewCustomer { get; set; }
    public IEnumerable<ScheduledItem>? Schedules { get; set; }
}

public enum Gender
{
    Male,
    Female
}
