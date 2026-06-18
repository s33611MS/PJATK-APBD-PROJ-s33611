using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Repositories.Agreement.Contract;

public interface IContractRepository
{
    Task<IEnumerable<Entities.Contract>> GetAllAsync(CancellationToken cancellationToken);
    Task<Entities.Contract?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Entities.Contract contract, CancellationToken cancellationToken);
    Task<int> DeleteAsync(int id, CancellationToken cancellationToken);
    Task CancelContractAsync(int id, CancellationToken cancellationToken);
    Task SignContractAsync(int id, CancellationToken cancellationToken);
    Task AddPaymentAsync(ContractPayment contractPayment, CancellationToken cancellationToken);
    Task<decimal> GetTotalPaymentsAsync(int contractId, CancellationToken cancellationToken);
}