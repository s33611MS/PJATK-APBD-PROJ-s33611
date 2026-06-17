using PJATK_APBD_PROJ_s33611.DTOs.Currency;
using PJATK_APBD_PROJ_s33611.Exceptions;

namespace PJATK_APBD_PROJ_s33611.Repositories;

public class NbpCurrencyRepository(HttpClient httpClient) : ICurrencyRepository
{
    public async Task<NbpResponseDto?> GetExchangeRateAsync(string currencyCode, CancellationToken cancellationToken)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<NbpResponseDto>(
                $"https://api.nbp.pl/api/exchangerates/rates/a/{currencyCode}",
                cancellationToken);
        }
        catch
        {
            throw new BadRequestException($"{currencyCode.ToUpper()} is not a valid currency code");
        }
    }
}