using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PJATK_APBD_PROJ_s33611.Entities;

[Table("Clients")]
public abstract class Client
{
    [Required, Key]
    public int Id { get; set; }
    [MaxLength(255)]
    public string Address { get; set; } = string.Empty;
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;
    [MaxLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;
}

[Table("IndividualClients")]
public class IndividualClient : Client
{
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
    [MaxLength(11)]
    public string Pesel { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
}

[Table("CompanyClients")]
public class CompanyClient : Client
{
    [MaxLength(255)]
    public string CompanyName { get; set; } = string.Empty;
    [MaxLength(10)]
    public string Krs { get; set; } = string.Empty;
}