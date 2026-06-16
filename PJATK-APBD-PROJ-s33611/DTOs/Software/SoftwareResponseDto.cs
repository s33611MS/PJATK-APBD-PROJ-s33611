namespace PJATK_APBD_PROJ_s33611.DTOs.Software;

public record SoftwareResponseDto(
    int Id,
    string Name,
    string Description,
    decimal LicensePricePerYear,
    SoftwareCategoryResponseDto SoftwareCategory
    );