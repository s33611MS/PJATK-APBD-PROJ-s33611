using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Repositories;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetAllAsync(CancellationToken cancellationToken);
    Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Client client, CancellationToken cancellationToken);
    Task UpdateAsync(Client client, CancellationToken cancellationToken);
    Task<bool> PeselExistsAsync(string pesel, CancellationToken cancellationToken);
    Task<bool> KrsExistsAsync(string krs, CancellationToken cancellationToken);
}