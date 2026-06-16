using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PJATK_APBD_PROJ_s33611.Entities;

[Table("SoftwareVersions")]
public class SoftwareVersion
{
    [Key]
    public int Id { get; set; }
    [MaxLength(20)]
    public string VersionNumber { get; set; } = string.Empty;
    public int SoftwareId { get; set; }
    public DateTime ReleaseDate { get; set; }
    public bool IsCurrent { get; set; }
    
    [ForeignKey(nameof(SoftwareId))]
    public Software Software { get; set; } = null!;
}