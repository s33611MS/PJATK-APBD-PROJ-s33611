using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PJATK_APBD_PROJ_s33611.Entities;

[Table("SubscriptionPayments")]
public class SubscriptionPayment
{
    [Key]
    public int Id { get; set; }
    [Column(TypeName =  "decimal(10,2)")]
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
    public DateTime StartDate { get; set; }
    public int SubscriptionId { get; set; }
    public int ClientId { get; set; }
    
    [ForeignKey(nameof(SubscriptionId))]
    public Subscription Subscription { get; set; } = null!;

    [ForeignKey(nameof(ClientId))]
    public Client Client { get; set; } = null!;
}