using PJATK_APBD_PROJ_s33611.DTOs.Client;
using PJATK_APBD_PROJ_s33611.DTOs.Software;
using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.DTOs.Contract;

public record ContractResponseDto(
    int Id,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal FinalPrice,
    string UpdatesInformation,
    int IncludedSupportYears,
    ContractStatus Status,
    ClientResponseDto Client,
    SoftwareResponseDto Software,
    SoftwareVersionResponseDto SoftwareVersion
    );