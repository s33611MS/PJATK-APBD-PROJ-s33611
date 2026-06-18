using Microsoft.EntityFrameworkCore;
using PJATK_APBD_PROJ_s33611.Data;

namespace PJATK_APBD_PROJ_s33611.Repositories.Client;

public class ClientRepository(DatabaseContext ctx) : IClientRepository
{
    public async Task<IEnumerable<Entities.Client>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await ctx.Clients
            .Where(c => c.IsDeleted == false)
            .ToListAsync(cancellationToken);
    }

    public async Task<Entities.Client?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await ctx.Clients
            .Where(c => c.Id == id && c.IsDeleted == false)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Entities.Client client, CancellationToken cancellationToken)
    {
        ctx.Add(client);
        await ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Entities.Client client, CancellationToken cancellationToken)
    {
        ctx.Update(client);
        await ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> PeselExistsAsync(string pesel, CancellationToken cancellationToken)
    {
        return await ctx.IndividualClients.AnyAsync(c => c.Pesel == pesel, cancellationToken);
    }

    public async Task<bool> KrsExistsAsync(string krs, CancellationToken cancellationToken)
    {
        return await ctx.CompanyClients.AnyAsync(c => c.Krs == krs, cancellationToken);
    }
}