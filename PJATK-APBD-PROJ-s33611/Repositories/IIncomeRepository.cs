using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Repositories;

public interface IIncomeRepository
{
    Task<decimal> GetAsync(int? softwareId, ContractStatus status, CancellationToken cancellationToken);
}