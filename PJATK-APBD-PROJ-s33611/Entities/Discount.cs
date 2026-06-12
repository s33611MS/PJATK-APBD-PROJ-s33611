using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PJATK_APBD_PROJ_s33611.Entities;

public enum DiscountTarget
{
    Contract,
    Subscription,
}

[Table("Discounts")]
public class Discount
{
    [Key]
    public int Id { get; set; }
    [MaxLength(200)]
    public string Name { get; set; } = null!;
    public DiscountTarget DiscountTarget { get; set; }
    [Range(0, 100)]
    public int Percentage { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidTo { get; set; }
    public int SoftwareId { get; set; }

    [ForeignKey(nameof(SoftwareId))]
    public Software Software { get; set; } = null!;
}