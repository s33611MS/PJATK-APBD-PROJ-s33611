using System.ComponentModel.DataAnnotations;

namespace PJATK_APBD_PROJ_s33611.DTOs.Subscription;

public record CreateSubscriptionDto(
    [Required]
    int ClientId,
    [Required]
    int SoftwareId,
    [Required]
    string Name,
    [Required]
    int RenewalPeriodInMonths,
    [Required]
    DateOnly StartDate
    );