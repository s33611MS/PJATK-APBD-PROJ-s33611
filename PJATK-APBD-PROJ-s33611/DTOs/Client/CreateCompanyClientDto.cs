using System.ComponentModel.DataAnnotations;

namespace PJATK_APBD_PROJ_s33611.DTOs.Client;

public record CreateCompanyClientDto(
    [Required]
    string CompanyName,
    [Required]
    string Krs,
    [Required]
    string Address,
    [Required]
    string Email,
    [Required]
    string PhoneNumber
    ) : CreateClientDto(Address, Email, PhoneNumber);