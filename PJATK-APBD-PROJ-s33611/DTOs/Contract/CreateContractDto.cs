using System.ComponentModel.DataAnnotations;
using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.DTOs.Contract;

public record CreateContractDto(
    [Required]
    int ClientId,
    [Required]
    int SoftwareId,
    [Required]
    int SoftwareVersionId,
    [Required]
    DateOnly StartDate,
    [Required]
    DateOnly EndDate,
    [Required]
    string UpdatesInformation,
    [Required]
    int AdditionalSupportYears
    );