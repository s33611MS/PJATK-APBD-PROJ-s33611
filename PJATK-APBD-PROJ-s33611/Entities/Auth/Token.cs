using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PJATK_APBD_PROJ_s33611.Entities.Auth;

[Table("Tokens")]
public class Token
{
    [Key]
    public int UserId { get; set; }
    [MaxLength(128)]
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public User User { get; set; } = null!;
}