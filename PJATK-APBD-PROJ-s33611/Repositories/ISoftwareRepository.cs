using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Repositories;

public interface ISoftwareRepository
{
    Task<Software?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> HasVersionAsync(int softwareId, int versionId, CancellationToken cancellationToken);
}