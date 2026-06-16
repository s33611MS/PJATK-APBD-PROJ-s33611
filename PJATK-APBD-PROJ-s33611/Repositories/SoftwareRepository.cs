using Microsoft.EntityFrameworkCore;
using PJATK_APBD_PROJ_s33611.Data;
using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Repositories;

public class SoftwareRepository(DatabaseContext ctx) : ISoftwareRepository
{
    public async Task<Software?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await ctx.Software
            .Where(s => s.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> HasVersionAsync(int softwareId, int versionId, CancellationToken cancellationToken)
    {
        return await ctx.SoftwareVersions.Where(sv => sv.Id == versionId && sv.SoftwareId == softwareId).AnyAsync(cancellationToken);
    }
}