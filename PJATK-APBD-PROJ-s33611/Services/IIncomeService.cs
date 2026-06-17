using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Services;

public interface IIncomeService
{
    Task<decimal> GetAsync(int? softwareId, ContractStatus status, CancellationToken cancellationToken);
}