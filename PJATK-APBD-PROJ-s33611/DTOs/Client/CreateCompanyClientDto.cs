namespace PJATK_APBD_PROJ_s33611.DTOs.Client;

public record CreateCompanyClientDto(
    string CompanyName,
    string Krs,
    string Address,
    string Email,
    string PhoneNumber
    );