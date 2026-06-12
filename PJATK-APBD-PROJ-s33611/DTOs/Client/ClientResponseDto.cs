using System.Text.Json.Serialization;

namespace PJATK_APBD_PROJ_s33611.DTOs.Client;

[JsonDerivedType(typeof(IndividualClientResponseDto))]
[JsonDerivedType(typeof(CompanyClientResponseDto))]
public abstract record ClientResponseDto(
    int Id,
    string Address,
    string Email,
    string PhoneNumber
    );