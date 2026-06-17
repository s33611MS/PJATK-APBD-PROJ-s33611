using PJATK_APBD_PROJ_s33611.Entities;
using PJATK_APBD_PROJ_s33611.Exceptions;
using PJATK_APBD_PROJ_s33611.Repositories;

namespace PJATK_APBD_PROJ_s33611.Services;

public class IncomeService(IIncomeRepository repo, ISoftwareRepository softwareRepository) : IIncomeService
{
    public async Task<decimal> GetAsync(int? softwareId, ContractStatus status, CancellationToken cancellationToken)
    {
        if (softwareId.HasValue)
        {
            if(await softwareRepository.GetByIdAsync(softwareId.GetValueOrDefault(), cancellationToken) == null)
                throw new NotFoundException($"There is no software with id {softwareId}");
        }

        return await repo.GetAsync(softwareId, status, cancellationToken);
    }
}