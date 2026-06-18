using PJATK_APBD_PROJ_s33611.Exceptions;
using PJATK_APBD_PROJ_s33611.Repositories;

namespace PJATK_APBD_PROJ_s33611.Services;

public class CurrencyService(ICurrencyRepository repo) : ICurrencyService
{
    public async Task<decimal> GetExchangeRateAsync(string currencyCode, CancellationToken cancellationToken)
    {
        if (currencyCode == "PLN")
            return 1;

        var code = currencyCode.ToLower();

        var response = await repo.GetExchangeRateAsync(code, cancellationToken);
        
        return response == null ? throw new NotFoundException($"Currency with code: {currencyCode.ToUpper()} was not found") : response.Rates.First().Mid;
    }
}