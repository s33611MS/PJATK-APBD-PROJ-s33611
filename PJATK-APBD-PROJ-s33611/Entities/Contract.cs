using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PJATK_APBD_PROJ_s33611.Entities;

public enum ContractStatus
{
    Draft,
    Signed,
    Cancelled
}

[Table("Contracts")]
public class Contract
{
    [Key]
    public int Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    [Column(TypeName =  "decimal(10,2)")]
    public decimal FinalPrice { get; set; }
    [MaxLength(100)]
    public string UpdatesInformation { get; set; } = string.Empty;
    public int IncludedSupportYears { get; set; } = 1;
    public ContractStatus Status { get; set; }
    public int ClientId { get; set; }
    public int SoftwareId { get; set; }
    public int SoftwareVersionId { get; set; }
    
    public ICollection<ContractPayment> ContractPayments { get; set; } = [];

    [ForeignKey(nameof(ClientId))]
    public Client Client { get; set; } = null!;

    [ForeignKey(nameof(SoftwareId))]
    public Software Software { get; set; } = null!;
    
    [ForeignKey(nameof(SoftwareVersionId))]
    public SoftwareVersion SoftwareVersion { get; set; } = null!;
}