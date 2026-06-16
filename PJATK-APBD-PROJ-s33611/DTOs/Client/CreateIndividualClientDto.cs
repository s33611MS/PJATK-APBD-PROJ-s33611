using System.ComponentModel.DataAnnotations;

namespace PJATK_APBD_PROJ_s33611.DTOs.Client;

public record CreateIndividualClientDto(
    [Required]
    string FirstName,
    [Required]
    string LastName,
    [Required]
    string Pesel,
    [Required]
    string Address,
    [Required]
    string Email,
    [Required]
    string PhoneNumber
    ) : CreateClientDto(Address, Email, PhoneNumber);