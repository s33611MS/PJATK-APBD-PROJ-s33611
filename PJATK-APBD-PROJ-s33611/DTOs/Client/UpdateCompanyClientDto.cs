namespace PJATK_APBD_PROJ_s33611.DTOs.Client;

public record UpdateCompanyClientDto(
    string CompanyName,
    string Address,
    string Email,
    string PhoneNumber
    ) : UpdateClientDto(Address, Email, PhoneNumber);