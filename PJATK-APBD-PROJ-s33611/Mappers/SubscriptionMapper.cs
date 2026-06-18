using PJATK_APBD_PROJ_s33611.DTOs.Software;
using PJATK_APBD_PROJ_s33611.DTOs.Subscription;
using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Mappers;

public class SubscriptionMapper
{
    public static SubscriptionResponseDto ToDto(Subscription subscription)
    {
        return new SubscriptionResponseDto(
            subscription.Id,
            subscription.Name,
            subscription.RenewalPeriodInMonths,
            subscription.Price,
            subscription.StartDate,
            subscription.NextRenewalDate,
            subscription.Status,
            ClientMapper.ToDto(subscription.Client),
            new SoftwareResponseDto(
                subscription.Software.Id,
                subscription.Software.Name,
                subscription.Software.Description,
                subscription.Software.LicensePricePerYear,
                new SoftwareCategoryResponseDto(
                    subscription.Software.SoftwareCategory.Id,
                    subscription.Software.SoftwareCategory.Name
                )
            )
        );
    }
    
    public static Subscription ToEntity(CreateSubscriptionDto dto, decimal finalPrice)
    {
        return new Subscription()
        {
            ClientId =  dto.ClientId,
            SoftwareId =  dto.SoftwareId,
            Name = dto.Name,
            RenewalPeriodInMonths = dto.RenewalPeriodInMonths,
            StartDate =  dto.StartDate,
            NextRenewalDate = dto.StartDate.AddMonths(dto.RenewalPeriodInMonths),
            Status = SubscriptionStatus.Active,
            Price =  finalPrice
        };
    }
}