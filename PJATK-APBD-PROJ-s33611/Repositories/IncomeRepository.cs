using Microsoft.EntityFrameworkCore;
using PJATK_APBD_PROJ_s33611.Data;
using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Repositories;

public class IncomeRepository(DatabaseContext ctx) : IIncomeRepository
{
    public async Task<decimal> GetAsync(int? softwareId, ContractStatus status, CancellationToken cancellationToken)
    {
        return await ctx.ContractPayments
            .Where(cp => (!softwareId.HasValue || cp.Contract.SoftwareId == softwareId) && (cp.Contract.Status == status || cp.Contract.Status == ContractStatus.Signed))
            .SumAsync(cp => cp.Amount,cancellationToken);
    }
}