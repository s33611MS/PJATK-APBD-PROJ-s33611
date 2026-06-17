using PJATK_APBD_PROJ_s33611.DTOs.Income;
using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Services;

public interface IIncomeService
{
    Task<IncomeResponseDto> GetAsync(int? softwareId, string? currencyCode, ContractStatus status, CancellationToken cancellationToken);
}