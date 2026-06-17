namespace PJATK_APBD_PROJ_s33611.DTOs.Income;

public record IncomeResponseDto(
    decimal Income,
    decimal ExchangeRate,
    string CurrencyCode
    );