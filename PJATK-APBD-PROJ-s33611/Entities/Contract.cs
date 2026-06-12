using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PJATK_APBD_PROJ_s33611.Entities;

[Table("Contracts")]
public class Contract
{
    [Key]
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    [Column(TypeName =  "decimal(10,2)")]
    public decimal FinalPrice { get; set; }
    [MaxLength(100)]
    public string UpdatesInformation { get; set; } = string.Empty;
    public int IncludedSupportYears { get; set; } = 1;
    public int ClientId { get; set; }
    public int SoftwareId { get; set; }
    public int SoftwareVersionId { get; set; }
    
    public IEnumerable<ContractPayment> ContractPayments { get; set; } = new List<ContractPayment>();

    [ForeignKey(nameof(ClientId))]
    public Client Client { get; set; } = null!;

    [ForeignKey(nameof(SoftwareId))]
    public Software Software { get; set; } = null!;
    
    [ForeignKey(nameof(SoftwareVersionId))]
    public SoftwareVersion SoftwareVersion { get; set; } = null!;
}