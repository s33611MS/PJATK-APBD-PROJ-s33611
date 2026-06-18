using PJATK_APBD_PROJ_s33611.DTOs.Contract;
using PJATK_APBD_PROJ_s33611.Entities;
using PJATK_APBD_PROJ_s33611.Exceptions;
using PJATK_APBD_PROJ_s33611.Mappers;
using PJATK_APBD_PROJ_s33611.Repositories.Agreement;
using PJATK_APBD_PROJ_s33611.Repositories.Agreement.Contract;
using PJATK_APBD_PROJ_s33611.Repositories.Client;
using PJATK_APBD_PROJ_s33611.Repositories.Software;

namespace PJATK_APBD_PROJ_s33611.Services.Contract;

public class ContractService(IContractRepository contractRepository, IAgreementRepository agreementRepository, IClientRepository clientRepository, ISoftwareRepository softwareRepository) : IContractService
{
    public async Task<IEnumerable<ContractResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var contracts = await contractRepository.GetAllAsync(cancellationToken);

        return contracts
            .Select(ContractMapper.ToDto)
            .ToList();
    }

    public async Task<ContractResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var contract = await contractRepository.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException($"There is no Contract with id: {id}");

        return ContractMapper.ToDto(contract);
    }

    public async Task<ContractResponseDto> AddAsync(CreateContractDto dto, CancellationToken cancellationToken)
    {
        var duration = dto.EndDate.DayNumber - dto.StartDate.DayNumber;
        if (!(duration >= 3 && duration <= 30))
            throw new BadRequestException("Contract's duration must be between 3 and 30 days");
        
        if (dto.AdditionalSupportYears < 0 || dto.AdditionalSupportYears > 3)
            throw new BadRequestException("Contract must have between 0 and 3 years of additional support years");
        
        var client = await clientRepository.GetByIdAsync(dto.ClientId, cancellationToken);
        if (client is null)
            throw new NotFoundException($"There is no Client with id: {dto.ClientId}");
        
        var software = await softwareRepository.GetByIdAsync(dto.SoftwareId, cancellationToken);
        if (software is null)
            throw new NotFoundException($"There is no Software with id: {dto.SoftwareId}");
        
        if (!await softwareRepository.HasVersionAsync(dto.SoftwareId, dto.SoftwareVersionId, cancellationToken))
            throw new NotFoundException($"There is no Software version with id: {dto.SoftwareVersionId} for Software with id: {dto.SoftwareId}");
        
        if(await agreementRepository.HasActiveContractOrSubscriptionForSoftwareAsync(dto.ClientId, dto.SoftwareId,dto.StartDate, cancellationToken))
            throw new ConflictException($"Client with id: {dto.ClientId} already has contract or subscription for software with id: {dto.SoftwareId}");
        
        var discount = await agreementRepository.GetBestDiscountAsync(DiscountType.Contract, cancellationToken);

        if (await agreementRepository.IsReturningClientAsync(dto.ClientId, cancellationToken))
            discount += 5;
        
        var softwarePrice = software.LicensePricePerYear;
        
        var finalPrice = (softwarePrice + dto.AdditionalSupportYears * 1000) * (100 - discount) / 100;
        
        var contract = ContractMapper.ToEntity(dto, finalPrice);

        await contractRepository.AddAsync(contract, cancellationToken);

        var created = await contractRepository.GetByIdAsync(contract.Id, cancellationToken);

        return ContractMapper.ToDto(created!);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var affectedRows = await contractRepository.DeleteAsync(id, cancellationToken);
        if (affectedRows == 0)
            throw new NotFoundException($"There is no Contract with id: {id}");
    }

    public async Task AddPaymentAsync(CreateContractPaymentDto dto, CancellationToken cancellationToken)
    {
        var contract = await contractRepository.GetByIdAsync(dto.ContractId, cancellationToken);

        if (contract is null)
            throw new NotFoundException($"There is no Contract with id: {dto.ContractId}");
        
        if (contract.Status == ContractStatus.Cancelled)
            throw new ConflictException($"Contract with id: {dto.ContractId} was cancelled");

        if (DateOnly.FromDateTime(DateTime.Today.Date) > contract.EndDate)
        {
            await contractRepository.CancelContractAsync(contract.Id, cancellationToken);
            throw new ConflictException("Contract expired");
        }

        var totalPaid = await contractRepository.GetTotalPaymentsAsync(contract.Id, cancellationToken);
        
        if (totalPaid == contract.FinalPrice)
            throw new ConflictException("Contract already paid");

        if (totalPaid + dto.Amount > contract.FinalPrice)
        {
            throw new BadRequestException($"Payment exceeds contract's price, {contract.FinalPrice - totalPaid} left to pay");
        }
        
        if (totalPaid + dto.Amount == contract.FinalPrice)
        {
            await contractRepository.SignContractAsync(contract.Id, cancellationToken);
        }
        
        var payment = new ContractPayment
        {
            ContractId = contract.Id,
            ClientId = contract.ClientId,
            Amount = dto.Amount,
            PaidAt = DateTime.Now
        };

        await contractRepository.AddPaymentAsync(payment, cancellationToken);
    }
}