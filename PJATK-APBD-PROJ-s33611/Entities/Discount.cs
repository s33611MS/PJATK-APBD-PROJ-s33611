using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PJATK_APBD_PROJ_s33611.Entities;

public enum DiscountType
{
    Contract,
    Subscription,
    Both
}

[Table("Discounts")]
public class Discount
{
    [Key]
    public int Id { get; set; }
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    public DiscountType Offer { get; set; }
    [Range(0, 100)]
    public int Percentage { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidTo { get; set; }
}