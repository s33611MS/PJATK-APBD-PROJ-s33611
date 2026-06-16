using System.ComponentModel.DataAnnotations;

namespace PJATK_APBD_PROJ_s33611.DTOs.Client;

public record UpdateClientDto(
    [Required]
    string Address,
    [Required]
    string Email,
    [Required]
    string PhoneNumber
    );