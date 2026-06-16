using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Repositories;

public interface IContractRepository
{
    Task<IEnumerable<Contract>> GetAllAsync(CancellationToken cancellationToken);
    Task<Contract?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Contract contract, CancellationToken cancellationToken);
    Task<int> DeleteAsync(int id, CancellationToken cancellationToken);
    Task CancelContractAsync(int id, CancellationToken cancellationToken);
    Task SignContractAsync(int id, CancellationToken cancellationToken);
    Task<int> GetBestDiscountAsync(DiscountType discountType, CancellationToken cancellationToken);
    Task<bool> IsReturningClientAsync(int clientId, CancellationToken cancellationToken);
    Task<bool> HasActiveContractForSoftwareAsync(int clientId, int softwareId, DateOnly startDate, CancellationToken cancellationToken);
    Task AddPaymentAsync(ContractPayment contractPayment, CancellationToken cancellationToken);
    Task<decimal> GetTotalPaymentsAsync(int contractId, CancellationToken cancellationToken);
}