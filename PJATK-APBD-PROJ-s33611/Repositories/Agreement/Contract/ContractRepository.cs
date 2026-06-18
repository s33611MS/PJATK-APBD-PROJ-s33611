using Microsoft.EntityFrameworkCore;
using PJATK_APBD_PROJ_s33611.Data;
using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Repositories.Agreement.Contract;

public class ContractRepository(DatabaseContext ctx) : IContractRepository
{
    public async Task<IEnumerable<Entities.Contract>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await ctx.Contracts
            .Include(c => c.Client)
            .Include(c => c.Software)
            .ThenInclude(s => s.SoftwareCategory)
            .Include(c => c.SoftwareVersion)
            .ToListAsync(cancellationToken);
    }

    public async Task<Entities.Contract?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await ctx.Contracts
            .Where(c => c.Id == id)
            .Include(c => c.Client)
            .Include(c => c.Software)
            .ThenInclude(s => s.SoftwareCategory)
            .Include(c => c.SoftwareVersion)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Entities.Contract contract, CancellationToken cancellationToken)
    {
        ctx.Add(contract);
        await ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var transaction = await ctx.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await ctx.ContractPayments
                .Where(cp => cp.ContractId == id)
                .ExecuteDeleteAsync(cancellationToken);
        
            var affectedRows = await ctx.Contracts
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
            return affectedRows;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task CancelContractAsync(int id, CancellationToken cancellationToken)
    {
        await ctx.ContractPayments.Where(cp => cp.ContractId == id)
            .ExecuteUpdateAsync(setters => 
                    setters.SetProperty(cp => cp.Status, PaymentStatus.Returned), 
                cancellationToken
            );
        
        await ctx.Contracts.Where(c => c.Id == id)
            .ExecuteUpdateAsync(setters => 
                    setters.SetProperty(c => c.Status, ContractStatus.Cancelled), 
                cancellationToken
            );
    }

    public async Task SignContractAsync(int id, CancellationToken cancellationToken)
    {
        await ctx.Contracts.Where(c => c.Id == id)
            .ExecuteUpdateAsync(setters => 
                    setters.SetProperty(c => c.Status, ContractStatus.Signed), 
                cancellationToken
            );
    }

    public async Task AddPaymentAsync(ContractPayment contractPayment, CancellationToken cancellationToken)
    {
        ctx.Add(contractPayment);
        await ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalPaymentsAsync(int contractId, CancellationToken cancellationToken)
    {
        return await ctx.ContractPayments
            .Where(cp => cp.ContractId == contractId && cp.Status == PaymentStatus.Accepted)
            .SumAsync(cp => cp.Amount, cancellationToken);
    }
}