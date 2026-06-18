using PJATK_APBD_PROJ_s33611.DTOs.Client;

namespace PJATK_APBD_PROJ_s33611.Services.Client;

public interface IClientService
{
    Task<IEnumerable<ClientResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<ClientResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ClientResponseDto> AddAsync(CreateClientDto dto, CancellationToken cancellationToken);
    Task UpdateAsync(int id, UpdateClientDto dto, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}