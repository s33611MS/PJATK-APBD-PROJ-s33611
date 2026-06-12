using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PJATK_APBD_PROJ_s33611.Entities;

public class Software
{
    [Key]
    public int Id { get; set; }
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    public SoftwareCategory Category { get; set; } = null!;
    [Column(TypeName =  "decimal(10,2)")]
    public decimal LicensePricePerYear { get; set; }

    public IEnumerable<SoftwareVersion> SoftwareVersions { get; set; } = [];
    
    public IEnumerable<Discount> Discounts { get; set; } = [];
}