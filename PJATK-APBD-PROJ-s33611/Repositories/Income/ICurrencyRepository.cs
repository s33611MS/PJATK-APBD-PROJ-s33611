using PJATK_APBD_PROJ_s33611.DTOs.Currency;

namespace PJATK_APBD_PROJ_s33611.Repositories.Income;

public interface ICurrencyRepository
{
    Task<NbpResponseDto?> GetExchangeRateAsync(string currencyCode, CancellationToken cancellationToken);
}