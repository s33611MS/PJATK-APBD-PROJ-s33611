namespace PJATK_APBD_PROJ_s33611.DTOs.Currency;

public record NbpResponseDto(
    List<NbpRate> Rates
    );
    
public record NbpRate(
    decimal Mid
    );