using PJATK_APBD_PROJ_s33611.DTOs.Client;
using PJATK_APBD_PROJ_s33611.DTOs.Software;
using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.DTOs.Subscription;

public record SubscriptionResponseDto(
    int Id,
    string Name,
    int RenewalPeriodInMonths,
    decimal Price,
    DateOnly StartDate,
    DateOnly NextRenewalDate,
    SubscriptionStatus Status,
    ClientResponseDto Client,
    SoftwareResponseDto Software
    );