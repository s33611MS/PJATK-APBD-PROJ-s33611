using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PJATK_APBD_PROJ_s33611.Entities;

[Table("SoftwareCategories")]
public class SoftwareCategory
{
    [Key]
    public int Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    
    public IEnumerable<Software> Software { get; set; } = new List<Software>();
}