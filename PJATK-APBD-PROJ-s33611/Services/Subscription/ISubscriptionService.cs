using PJATK_APBD_PROJ_s33611.DTOs.Subscription;

namespace PJATK_APBD_PROJ_s33611.Services.Subscription;

public interface ISubscriptionService
{
    Task<IEnumerable<SubscriptionResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<SubscriptionResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<SubscriptionResponseDto> AddAsync(CreateSubscriptionDto dto, CancellationToken cancellationToken);
    Task AddPaymentAsync(CreateSubscriptionPaymentDto dto, CancellationToken cancellationToken);
}