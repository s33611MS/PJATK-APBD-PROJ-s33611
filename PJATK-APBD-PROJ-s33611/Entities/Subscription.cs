using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PJATK_APBD_PROJ_s33611.Entities;

public enum SubscriptionStatus
{
    Active,
    Cancelled
}

[Table("Subscriptions")]
public class Subscription
{
    [Key]
    public int Id { get; set; }
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    public int RenewalPeriodInMonths { get; set; }
    [Column(TypeName =  "decimal(10,2)")]
    public decimal Price { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly NextRenewalDate { get; set; }
    public SubscriptionStatus Status { get; set; }
    public int ClientId { get; set; }
    public int SoftwareId { get; set; }
    
    [ForeignKey(nameof(ClientId))]
    public Client Client { get; set; } = null!;
    
    [ForeignKey(nameof(SoftwareId))]
    public Software Software { get; set; } = null!;

    public ICollection<SubscriptionPayment> Payments { get; set; } = [];
}