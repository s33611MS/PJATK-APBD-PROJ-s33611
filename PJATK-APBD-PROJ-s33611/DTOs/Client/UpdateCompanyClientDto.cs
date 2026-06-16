using System.ComponentModel.DataAnnotations;

namespace PJATK_APBD_PROJ_s33611.DTOs.Client;

public record UpdateCompanyClientDto(
    [Required]
    string CompanyName,
    [Required]
    string Address,
    [Required]
    string Email,
    [Required]
    string PhoneNumber
    ) : UpdateClientDto(Address, Email, PhoneNumber);