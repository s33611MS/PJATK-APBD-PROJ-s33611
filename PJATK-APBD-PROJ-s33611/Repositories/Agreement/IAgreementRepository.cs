using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Repositories.Agreement;

public interface IAgreementRepository
{
    Task<int> GetBestDiscountAsync(DiscountType type, CancellationToken cancellationToken);
    Task<bool> IsReturningClientAsync(int clientId, CancellationToken cancellationToken);
    Task<bool> HasActiveContractOrSubscriptionForSoftwareAsync(int clientId, int softwareId, DateOnly startDate, CancellationToken cancellationToken);
}