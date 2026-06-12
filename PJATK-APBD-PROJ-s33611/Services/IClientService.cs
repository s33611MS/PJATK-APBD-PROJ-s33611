using PJATK_APBD_PROJ_s33611.DTOs;
using PJATK_APBD_PROJ_s33611.DTOs.Client;

namespace PJATK_APBD_PROJ_s33611.Services;

public interface IClientService
{
    Task<IEnumerable<ClientResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<ClientResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<int> AddIndividualAsync(CreateIndividualClientDto dto, CancellationToken cancellationToken);
    Task<int> AddCompanyAsync(CreateCompanyClientDto dto, CancellationToken cancellationToken);
    Task UpdateIndividualAsync(int id, UpdateIndividualClientDto dto, CancellationToken cancellationToken);
    Task UpdateCompanyAsync(int id, UpdateCompanyClientDto dto, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}