using PJATK_APBD_PROJ_s33611.DTOs.Income;
using PJATK_APBD_PROJ_s33611.Entities;
using PJATK_APBD_PROJ_s33611.Exceptions;
using PJATK_APBD_PROJ_s33611.Repositories.Income;
using PJATK_APBD_PROJ_s33611.Repositories.Software;

namespace PJATK_APBD_PROJ_s33611.Services.Income;

public class IncomeService(IIncomeRepository repo, ICurrencyService currencyService, ISoftwareRepository softwareRepository) : IIncomeService
{
    public async Task<IncomeResponseDto> GetAsync(int? softwareId, string? currencyCode, bool expected, CancellationToken cancellationToken)
    {
        if (softwareId.HasValue)
        {
            if(await softwareRepository.GetByIdAsync(softwareId.GetValueOrDefault(), cancellationToken) == null)
                throw new NotFoundException($"There is no software with id {softwareId}");
        }
        
        currencyCode ??= "PLN";
        
        var raw = await repo.GetAsync(softwareId, expected, cancellationToken);
        var currencyRate = await currencyService.GetExchangeRateAsync(currencyCode, cancellationToken);
        
        var result = Math.Round(raw/currencyRate, 2);

        return new IncomeResponseDto(result, currencyRate, currencyCode.ToUpper());
    }
}