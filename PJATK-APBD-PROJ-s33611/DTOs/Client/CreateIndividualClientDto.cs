namespace PJATK_APBD_PROJ_s33611.DTOs.Client;

public record CreateIndividualClientDto(
    string FirstName,
    string LastName,
    string Pesel,
    string Address,
    string Email,
    string PhoneNumber
    ) : CreateClientDto(Address, Email, PhoneNumber);