using Microsoft.EntityFrameworkCore;
using PJATK_APBD_PROJ_s33611.Data;
using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Repositories;

public class ClientRepository(DatabaseContext ctx) : IClientRepository
{
    public async Task<IEnumerable<Client>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await ctx.Clients
            .Where(c => c.IsDeleted == false)
            .ToListAsync(cancellationToken);
    }

    public async Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await ctx.Clients
            .Where(c => c.Id == id && c.IsDeleted == false)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Client request, CancellationToken cancellationToken)
    {
        ctx.Add(request);
        await ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Client request, CancellationToken cancellationToken)
    {
        ctx.Update(request);
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