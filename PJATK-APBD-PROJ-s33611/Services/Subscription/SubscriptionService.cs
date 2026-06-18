using PJATK_APBD_PROJ_s33611.DTOs.Subscription;
using PJATK_APBD_PROJ_s33611.Entities;
using PJATK_APBD_PROJ_s33611.Exceptions;
using PJATK_APBD_PROJ_s33611.Mappers;
using PJATK_APBD_PROJ_s33611.Repositories.Agreement;
using PJATK_APBD_PROJ_s33611.Repositories.Agreement.Subscription;
using PJATK_APBD_PROJ_s33611.Repositories.Client;
using PJATK_APBD_PROJ_s33611.Repositories.Software;

namespace PJATK_APBD_PROJ_s33611.Services.Subscription;

public class SubscriptionService(ISubscriptionRepository subscriptionRepository, IAgreementRepository agreementRepository, IClientRepository clientRepository, ISoftwareRepository softwareRepository) : ISubscriptionService
{
    public async Task<IEnumerable<SubscriptionResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var subscriptions = await subscriptionRepository.GetAllAsync(cancellationToken);

        return subscriptions
            .Select(SubscriptionMapper.ToDto)
            .ToList();
    }

    public async Task<SubscriptionResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var subscription = await subscriptionRepository.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException($"There is no Contract with id: {id}");

        return SubscriptionMapper.ToDto(subscription);
    }

    public async Task<SubscriptionResponseDto> AddAsync(CreateSubscriptionDto dto, CancellationToken cancellationToken)
    {
        if (dto.RenewalPeriodInMonths < 1 || dto.RenewalPeriodInMonths > 24)
            throw new BadRequestException("Subscription renewal period must be between 1 month and 2 years");
        
        var client = await clientRepository.GetByIdAsync(dto.ClientId, cancellationToken);
        if (client is null)
            throw new NotFoundException($"There is no Client with id: {dto.ClientId}");
        
        var software = await softwareRepository.GetByIdAsync(dto.SoftwareId, cancellationToken);
        if (software is null)
            throw new NotFoundException($"There is no Software with id: {dto.SoftwareId}");
        
        if(await agreementRepository.HasActiveContractOrSubscriptionForSoftwareAsync(dto.ClientId, dto.SoftwareId,dto.StartDate, cancellationToken))
            throw new ConflictException($"Client with id: {dto.ClientId} already has contract or subscription for software with id: {dto.SoftwareId}");
        
        var softwarePrice = software.LicensePricePerYear / 12 * dto.RenewalPeriodInMonths;
        
        var subscription = SubscriptionMapper.ToEntity(dto, softwarePrice);
        
        var discount = await agreementRepository.GetBestDiscountAsync(DiscountType.Subscription, cancellationToken);
        
        if (await agreementRepository.IsReturningClientAsync(dto.ClientId, cancellationToken))
            discount += 5;
        
        var finalPrice = softwarePrice * (100 - discount) / 100;
        
        subscription.Payments.Add(new SubscriptionPayment
        {
            Amount = finalPrice,
            ClientId = subscription.ClientId,
            PaidAt = DateTime.Now
        });

        await subscriptionRepository.AddAsync(subscription, cancellationToken);

        var created = await subscriptionRepository.GetByIdAsync(subscription.Id, cancellationToken);

        return SubscriptionMapper.ToDto(created!);
    }

    public async Task AddPaymentAsync(CreateSubscriptionPaymentDto dto, CancellationToken cancellationToken)
    {
        var subscription = await subscriptionRepository.GetByIdAsync(dto.SubscriptionId, cancellationToken);

        if (subscription is null)
            throw new NotFoundException($"There is no Subscription with id: {dto.SubscriptionId}");
        
        if (subscription.Status == SubscriptionStatus.Cancelled)
            throw new ConflictException($"Subscription with id: {dto.SubscriptionId} was cancelled");

        if (DateOnly.FromDateTime(DateTime.Today.Date) > subscription.NextRenewalDate.AddMonths(subscription.RenewalPeriodInMonths))
        {
            await subscriptionRepository.CancelSubscriptionAsync(subscription.Id, cancellationToken);
            throw new ConflictException("Payment after renewal period, subscription cancelled");
        }
        
        if (DateOnly.FromDateTime(DateTime.Today.Date) <  subscription.NextRenewalDate)
            throw new ConflictException($"Subscription with id: {dto.SubscriptionId} was already paid for");

        var price = subscription.Price;
        
        if (await agreementRepository.IsReturningClientAsync(subscription.ClientId, cancellationToken))
            price = Math.Round(price * 95 / 100, 2);
        
        if (dto.Amount != price)
            throw new BadRequestException($"Subscription costs {price}, instead got {dto.Amount}");
        
        var payment = new SubscriptionPayment
        {
            SubscriptionId = subscription.Id,
            ClientId = subscription.ClientId,
            Amount = dto.Amount,
            PaidAt = DateTime.Now
        };

        await subscriptionRepository.AddPaymentAsync(payment, cancellationToken);
    }
}