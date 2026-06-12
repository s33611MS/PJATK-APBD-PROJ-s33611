namespace PJATK_APBD_PROJ_s33611.DTOs.Client;

public record CompanyClientResponseDto(
    int Id,
    string Address,
    string Email,
    string PhoneNumber,
    string CompanyName,
    string Krs
    ) : ClientResponseDto(Id, Address, Email, PhoneNumber);