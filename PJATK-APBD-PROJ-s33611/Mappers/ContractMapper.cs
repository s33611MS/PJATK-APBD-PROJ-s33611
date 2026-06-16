using PJATK_APBD_PROJ_s33611.DTOs.Client;
using PJATK_APBD_PROJ_s33611.DTOs.Contract;
using PJATK_APBD_PROJ_s33611.DTOs.Software;
using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Mappers;

public static class ContractMapper
{
    public static ContractResponseDto ToDto(Contract contract)
    {
        return new ContractResponseDto(
            contract.Id,
            contract.StartDate,
            contract.EndDate,
            contract.FinalPrice,
            contract.UpdatesInformation,
            contract.IncludedSupportYears,
            contract.Status,
            ClientMapper.ToDto(contract.Client),
            new SoftwareResponseDto(
                contract.Software.Id,
                contract.Software.Name,
                contract.Software.Description,
                contract.Software.LicensePricePerYear,
                new SoftwareCategoryResponseDto(
                    contract.Software.SoftwareCategory.Id,
                    contract.Software.SoftwareCategory.Name
                    )
                ), 
            new SoftwareVersionResponseDto(
                contract.SoftwareVersion.Id,
                contract.SoftwareVersion.VersionNumber,
                contract.SoftwareVersion.ReleaseDate
            )
        );
    }
    
    public static Contract ToEntity(CreateContractDto dto, decimal finalPrice)
    {
        return new Contract
        {
            ClientId = dto.ClientId,
            SoftwareId = dto.SoftwareId,
            SoftwareVersionId = dto.SoftwareVersionId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            FinalPrice = finalPrice,
            UpdatesInformation = dto.UpdatesInformation,
            IncludedSupportYears = 1 + dto.AdditionalSupportYears,
            Status = ContractStatus.Draft
        };
    }
}