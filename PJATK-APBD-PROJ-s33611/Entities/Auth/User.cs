using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PJATK_APBD_PROJ_s33611.Entities.Auth;

[Table("AuthUsers")]
public class User
{
    [Key]
    public int Id { get; set; }
    [MaxLength(100)]
    public string Login { get; set; } = string.Empty;
    [MaxLength(256)]
    public string PasswordHash { get; set; } = string.Empty;
    public int RoleId { get; set; }
    
    [ForeignKey(nameof(RoleId))]
    public UserRole UserRole { get; set; } = null!;
    public Token? Token { get; set; }
}