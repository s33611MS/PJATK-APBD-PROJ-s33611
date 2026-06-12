namespace PJATK_APBD_PROJ_s33611.DTOs.Client;

public record UpdateIndividualClientDto(
    string FirstName,
    string LastName,
    string Address,
    string Email,
    string PhoneNumber
    );