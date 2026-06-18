namespace PJATK_APBD_PROJ_s33611.Services.Income;

public interface ICurrencyService
{
    Task<decimal> GetExchangeRateAsync(string currencyCode, CancellationToken cancellationToken);
}