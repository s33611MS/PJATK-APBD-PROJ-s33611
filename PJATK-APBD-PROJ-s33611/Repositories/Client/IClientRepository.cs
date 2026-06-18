namespace PJATK_APBD_PROJ_s33611.Repositories.Client;

public interface IClientRepository
{
    Task<IEnumerable<Entities.Client>> GetAllAsync(CancellationToken cancellationToken);
    Task<Entities.Client?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Entities.Client client, CancellationToken cancellationToken);
    Task UpdateAsync(Entities.Client client, CancellationToken cancellationToken);
    Task<bool> PeselExistsAsync(string pesel, CancellationToken cancellationToken);
    Task<bool> KrsExistsAsync(string krs, CancellationToken cancellationToken);
}