using Microsoft.EntityFrameworkCore;
using PJATK_APBD_PROJ_s33611.Data;
using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Repositories.Income;

public class IncomeRepository(DatabaseContext ctx) : IIncomeRepository
{
    public async Task<decimal> GetAsync(int? softwareId, bool expected, CancellationToken cancellationToken)
    {
        var subscriptionsExpected = 0m;
        var contractsExpected = 0m;
        if (expected)
        {
            contractsExpected = await ctx.Contracts
                .Where(c => (!softwareId.HasValue || c.SoftwareId == softwareId) && c.Status == ContractStatus.Draft)
                .SumAsync(c => c.FinalPrice, cancellationToken);
            
            subscriptionsExpected = await  ctx.Subscriptions
                .Where(s => (!softwareId.HasValue || s.SoftwareId == softwareId) && s.Status == SubscriptionStatus.Active)
                .SumAsync(s => s.Price * 95 / 100, cancellationToken);
            
        }
        var contractsPaid = await ctx.ContractPayments
            .Where(cp => (!softwareId.HasValue || cp.Contract.SoftwareId == softwareId) && cp.Contract.Status == ContractStatus.Signed)
            .SumAsync(cp => cp.Amount, cancellationToken);
        
        var subscriptionsPaid = await ctx.SubscriptionPayments
            .Where(sp => (!softwareId.HasValue || sp.Subscription.SoftwareId == softwareId) && sp.Subscription.Status == SubscriptionStatus.Active)
            .SumAsync(sp => sp.Amount, cancellationToken);
        
        Console.WriteLine(contractsPaid);
        Console.WriteLine(contractsExpected);
        Console.WriteLine(subscriptionsPaid);
        Console.WriteLine(subscriptionsExpected);
        
        return contractsPaid + contractsExpected + subscriptionsPaid + subscriptionsExpected;
    }
}