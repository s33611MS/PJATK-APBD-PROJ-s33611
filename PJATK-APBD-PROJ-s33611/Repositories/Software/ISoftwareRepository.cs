namespace PJATK_APBD_PROJ_s33611.Repositories.Software;

public interface ISoftwareRepository
{
    Task<Entities.Software?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> HasVersionAsync(int softwareId, int versionId, CancellationToken cancellationToken);
}