namespace PJATK_APBD_PROJ_s33611.DTOs.Client;

public record IndividualClientResponseDto(
    int Id,
    string Address,
    string Email,
    string PhoneNumber,
    string FirstName,
    string LastName,
    string Pesel
) : ClientResponseDto(Id, Address, Email, PhoneNumber);