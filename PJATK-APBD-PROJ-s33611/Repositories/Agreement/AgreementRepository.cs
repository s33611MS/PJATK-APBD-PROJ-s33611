using Microsoft.EntityFrameworkCore;
using PJATK_APBD_PROJ_s33611.Data;
using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Repositories.Agreement;

public class AgreementRepository(DatabaseContext ctx) : IAgreementRepository
{
    public async Task<int> GetBestDiscountAsync(DiscountType type, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Today.Date);
        var discounts = await ctx.Discounts
            .Where(d => (d.Offer == type || d.Offer == DiscountType.Both) 
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

    public async Task<bool> HasActiveContractOrSubscriptionForSoftwareAsync(int clientId, int softwareId, DateOnly startDate, CancellationToken cancellationToken)
    {
        var hasContract = await ctx.Contracts.AnyAsync(c => c.ClientId == clientId && c.SoftwareId == softwareId && c.EndDate > startDate, cancellationToken);
        var hasSubscription = await ctx.Subscriptions.AnyAsync(s => s.ClientId == clientId && s.Status == SubscriptionStatus.Active, cancellationToken);
        
        return hasContract || hasSubscription;
    }
}