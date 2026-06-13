using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Repositories;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetAllAsync(CancellationToken cancellationToken);
    Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Client request, CancellationToken cancellationToken);
    Task UpdateAsync(Client request, CancellationToken cancellationToken);
    Task<bool> PeselExistsAsync(string pesel, CancellationToken cancellationToken);
    Task<bool> KrsExistsAsync(string krs, CancellationToken cancellationToken);
}