using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PJATK_APBD_PROJ_s33611.Entities;

public enum PaymentStatus
{
    Accepted,
    Returned
}

[Table("ContractPayments"), PrimaryKey(nameof(ContractId), nameof(ClientId), nameof(PaidAt))]
public class ContractPayment
{
    [Column(TypeName =  "decimal(10,2)")]
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Accepted;
    public int ContractId { get; set; }
    public int ClientId { get; set; }

    [ForeignKey(nameof(ContractId))]
    public Contract Contract { get; set; } = null!;
    
    [ForeignKey(nameof(ClientId))]
    public Client Client { get; set; } = null!;
}