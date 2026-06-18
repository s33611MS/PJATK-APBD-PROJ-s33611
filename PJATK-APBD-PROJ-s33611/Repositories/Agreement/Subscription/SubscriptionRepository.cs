using Microsoft.EntityFrameworkCore;
using PJATK_APBD_PROJ_s33611.Data;
using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Repositories.Agreement.Subscription;

public class SubscriptionRepository(DatabaseContext ctx) : ISubscriptionRepository
{
    public async Task<IEnumerable<Entities.Subscription>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await ctx.Subscriptions
            .Include(s => s.Client)
            .Include(s => s.Software)
            .ThenInclude(so => so.SoftwareCategory)
            .ToListAsync(cancellationToken);
    }

    public async Task<Entities.Subscription?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await ctx.Subscriptions
            .Where(s => s.Id == id)
            .Include(s => s.Client)
            .Include(s => s.Software)
            .ThenInclude(so => so.SoftwareCategory)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Entities.Subscription subscription, CancellationToken cancellationToken)
    {
        ctx.Add(subscription);
        await ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task CancelSubscriptionAsync(int id, CancellationToken cancellationToken)
    {
        await ctx.Subscriptions.Where(s => s.Id == id)
            .ExecuteUpdateAsync(setters => 
                    setters.SetProperty(s => s.Status, SubscriptionStatus.Cancelled), 
                cancellationToken
            );
    }

    public async Task AddPaymentAsync(SubscriptionPayment subscriptionPayment, CancellationToken cancellationToken)
    {
        ctx.SubscriptionPayments.Add(subscriptionPayment);

        var subscription = await GetByIdAsync(subscriptionPayment.SubscriptionId, cancellationToken);

        subscription!.NextRenewalDate = subscription.NextRenewalDate.AddMonths(subscription.RenewalPeriodInMonths);

        await ctx.SaveChangesAsync(cancellationToken);
    }
}