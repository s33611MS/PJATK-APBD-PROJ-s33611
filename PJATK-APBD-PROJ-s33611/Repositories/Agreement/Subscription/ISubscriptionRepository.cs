using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Repositories.Agreement.Subscription;

public interface ISubscriptionRepository
{
    Task<IEnumerable<Entities.Subscription>> GetAllAsync(CancellationToken cancellationToken);
    Task<Entities.Subscription?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Entities.Subscription contract, CancellationToken cancellationToken);
    Task CancelSubscriptionAsync(int id, CancellationToken cancellationToken);
    Task AddPaymentAsync(SubscriptionPayment subscriptionPayment, CancellationToken cancellationToken);
}