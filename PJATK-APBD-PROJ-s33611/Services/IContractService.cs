using PJATK_APBD_PROJ_s33611.DTOs.Contract;

namespace PJATK_APBD_PROJ_s33611.Services;

public interface IContractService
{
    Task<IEnumerable<ContractResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<ContractResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ContractResponseDto> AddAsync(CreateContractDto dto, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task AddPaymentAsync(CreateContractPaymentDto dto, CancellationToken cancellationToken);
}