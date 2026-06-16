using Microsoft.EntityFrameworkCore;
using PJATK_APBD_PROJ_s33611.Data;
using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Repositories;

public class ContractRepository(DatabaseContext ctx) : IContractRepository
{
    public async Task<IEnumerable<Contract>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await ctx.Contracts
            .Include(x => x.Client)
            .Include(x => x.Software)
            .ThenInclude(x => x.SoftwareCategory)
            .Include(x => x.SoftwareVersion)
            .ToListAsync(cancellationToken);
    }

    public async Task<Contract?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await ctx.Contracts
            .Where(c => c.Id == id)
            .Include(x => x.Client)
            .Include(x => x.Software)
            .ThenInclude(x => x.SoftwareCategory)
            .Include(x => x.SoftwareVersion)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Contract contract, CancellationToken cancellationToken)
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

    public async Task<int> GetBestDiscountAsync(DiscountType discountType, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Today.Date);
        var discounts = await ctx.Discounts
            .Where(d => (d.Offer == discountType || d.Offer == DiscountType.Both) 
                        && today >= d.ValidFrom 
                        && today <= d.ValidTo)
            .Select(d => d.Percentage)
            .ToListAsync(cancellationToken);
        
        return discounts.Count != 0 ? discounts.Max() : 0;
    }

    public async Task<bool> IsReturningClientAsync(int clientId, CancellationToken cancellationToken)
    {
        return await ctx.Contracts.AnyAsync(c => c.ClientId == clientId && c.Status == ContractStatus.Signed, cancellationToken) || 
               await ctx.Subscriptions.AnyAsync(s => s.ClientId == clientId, cancellationToken);
    }

    public async Task<bool> HasActiveContractForSoftwareAsync(int clientId, int softwareId, DateOnly startDate, CancellationToken cancellationToken)
    {
        return await ctx.Contracts.AnyAsync(c => c.ClientId == clientId && c.SoftwareId == softwareId && c.EndDate > startDate, cancellationToken);
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