using FluentAssertions;
using Moq;
using PJATK_APBD_PROJ_s33611.DTOs.Contract;
using PJATK_APBD_PROJ_s33611.Entities;
using PJATK_APBD_PROJ_s33611.Exceptions;
using PJATK_APBD_PROJ_s33611.Repositories;
using PJATK_APBD_PROJ_s33611.Repositories.Agreement;
using PJATK_APBD_PROJ_s33611.Repositories.Agreement.Contract;
using PJATK_APBD_PROJ_s33611.Repositories.Client;
using PJATK_APBD_PROJ_s33611.Repositories.Software;
using PJATK_APBD_PROJ_s33611.Services;
using PJATK_APBD_PROJ_s33611.Services.Contract;

namespace TestProject;

public class ContractServiceTests
{
    private readonly Mock<IContractRepository> _contractRepository = new();
    private readonly Mock<IAgreementRepository> _agreementRepository = new();
    private readonly Mock<IClientRepository> _clientRepository = new();
    private readonly Mock<ISoftwareRepository> _softwareRepository = new();

    private ContractService CreateService()
    {
        return new ContractService(
            _contractRepository.Object,
            _agreementRepository.Object,
            _clientRepository.Object,
            _softwareRepository.Object
            );
    }
    
    private CreateContractDto ValidDto()
    {
        return new CreateContractDto(
            ClientId: 1,
            SoftwareId: 1,
            SoftwareVersionId: 1,
            StartDate: new DateOnly(2026, 1, 1),
            EndDate: new DateOnly(2026, 1, 10),
            UpdatesInformation:"",
            AdditionalSupportYears: 0
        );
    }

    private readonly Client _client = new IndividualClient
    {
        Id = 1,
        FirstName = "Jan",
        LastName = "Kowalski",
        Pesel = "12345678901",
        Address = "Ulica 1",
        Email = "test@test.pl",
        PhoneNumber = "123456789"
    };
    private readonly Software _software = new()
    {
        Id = 1,
        Name = "Software 1",
        Description = "Software 1 description",
        LicensePricePerYear = 1000,
        SoftwareCategory = new SoftwareCategory
        {
            Id = 1,
            Name = "Software Category"
        }
    };
    private readonly Contract _contract = new()
    {
        Id = 1,
        FinalPrice = 1000,
        EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1))
    };
    
    [Fact]
    public async Task GetById_WhenContractDoesNotExist_ShouldThrow()
    {
        var service = CreateService();
        
        _contractRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<int>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contract?)null);
        
        Func<Task> action = () => service.GetByIdAsync(1, CancellationToken.None);
        
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddContract_WhenDurationIsLessThan3DaysOrMoreThan30Days_ShouldThrow()
    {
        var dto = new CreateContractDto(
            ClientId: 1,
            SoftwareId: 1,
            SoftwareVersionId: 1,
            StartDate: new DateOnly(2026, 1, 1),
            EndDate: new DateOnly(2026, 1, 2),
            UpdatesInformation:"",
            AdditionalSupportYears: 0
        );

        var service = CreateService();
        
        Func<Task> action = async () => await service.AddAsync(dto, CancellationToken.None);
        
        await action.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("Contract's duration must be between 3 and 30 days");
    }
    
    [Fact]
    public async Task AddContract_WhenAdditionalSupportYearsNotBetween0And3_ShouldThrow()
    {
        var dto = new CreateContractDto(
            ClientId: 1,
            SoftwareId: 1,
            SoftwareVersionId: 1,
            StartDate: new DateOnly(2026, 1, 1),
            EndDate: new DateOnly(2026, 1, 10),
            UpdatesInformation:"",
            AdditionalSupportYears: 4
        );

        var service = CreateService();
        
        Func<Task> action = async () => await service.AddAsync(dto, CancellationToken.None);
        
        await action.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("Contract must have between 0 and 3 years of additional support years");
    }
    
    [Fact]
    public async Task AddContract_WhenClientDoesNotExist_ShouldThrow()
    {
        var dto = ValidDto();

        var service = CreateService();
        
        _clientRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);
        
        Func<Task> action = () => service.AddAsync(dto, CancellationToken.None);
        
        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"There is no Client with id: {dto.ClientId}");
    }
    
    [Fact]
    public async Task AddContract_WhenSoftwareDoesNotExist_ShouldThrow()
    {
        var dto = ValidDto();

        var service = CreateService();
        
        _clientRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_client);
        
        _softwareRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Software?)null);
        
        Func<Task> action = () => service.AddAsync(dto, CancellationToken.None);
        
        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"There is no Software with id: {dto.SoftwareId}");
    }
    
    [Fact]
    public async Task AddContract_WhenSoftwareVersionDoesNotMatchSoftware_ShouldThrow()
    {
        var dto = ValidDto();

        var service = CreateService();
        
        _clientRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_client);
        
        _softwareRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_software);
        
        _softwareRepository
            .Setup(x => x.HasVersionAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        Func<Task> action = () => service.AddAsync(dto, CancellationToken.None);
        
        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"There is no Software version with id: {dto.SoftwareVersionId} for Software with id: {dto.SoftwareId}");
    }
    
    [Fact]
    public async Task AddContract_WhenActiveContractExists_ShouldThrow()
    {
        var dto = ValidDto();

        var service = CreateService();
        
        _clientRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_client);

        _softwareRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_software);

        _softwareRepository
            .Setup(x => x.HasVersionAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _agreementRepository
            .Setup(x => x.HasActiveContractOrSubscriptionForSoftwareAsync(1, 1, new  DateOnly(2026, 1, 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        Func<Task> action = () => service.AddAsync(dto, CancellationToken.None);
        
        await action.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage($"Client with id:{dto.ClientId} already has a contract or subscription for software with id:{dto.SoftwareId}");
    }
    
    [Fact]
    public async Task AddContract_ShouldApplyDiscounts()
{
    var dto = ValidDto();

    var version = new SoftwareVersion
    {
        Id = 1,
        VersionNumber = "1.0",
        ReleaseDate = new DateTime(2026, 1, 1)
    };
    
    var service = CreateService();

    _clientRepository
        .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
        .ReturnsAsync(_client);

    _softwareRepository
        .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
        .ReturnsAsync(_software);

    _softwareRepository
        .Setup(x => x.HasVersionAsync(1, 1, It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);

    _agreementRepository
        .Setup(x => x.HasActiveContractOrSubscriptionForSoftwareAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(false);

    _agreementRepository
        .Setup(x => x.GetBestDiscountAsync(DiscountType.Contract, It.IsAny<CancellationToken>()))
        .ReturnsAsync(10);

    _agreementRepository
        .Setup(x => x.IsReturningClientAsync(1, It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);

    Contract? savedContract = null;

    _contractRepository
        .Setup(x => x.AddAsync(It.IsAny<Contract>(), It.IsAny<CancellationToken>()))
        .Callback<Contract, CancellationToken>((c, _) =>
        {
            c.Id = 1;
            savedContract = c;
        });

    _contractRepository
        .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
        .ReturnsAsync(() =>
        {
            savedContract!.Client = _client;
            savedContract.Software = _software;
            savedContract.SoftwareVersion = version;

            return savedContract;
        });
    
    var result = await service.AddAsync(dto, CancellationToken.None);

    result.FinalPrice.Should().Be(850);

    _contractRepository.Verify(x => x.AddAsync(It.IsAny<Contract>(), It.IsAny<CancellationToken>()), Times.Once);
}
    
    [Fact]
    public async Task AddPayment_WhenFullyPaid_ShouldSignContract()
    {
        var service = CreateService();
        
        _contractRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_contract);

        _contractRepository
            .Setup(x => x.GetTotalPaymentsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(800);
        
        await service.AddPaymentAsync(new CreateContractPaymentDto(1, 200), CancellationToken.None);
        
        _contractRepository.Verify(x => x.SignContractAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task AddPayment_ForCancelledContract_ShouldThrow()
    {
        var contract = new Contract
        {
            Id = 1,
            Status = ContractStatus.Cancelled
        };
        
        var service = CreateService();

        _contractRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        Func<Task> action = () => service.AddPaymentAsync(new CreateContractPaymentDto(1, 100), CancellationToken.None);

        await action.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage($"Contract with id: {contract.Id} was cancelled");
    }
    
    [Fact]
    public async Task AddPayment_AfterDeadline_ShouldThrow()
    {
        var contract = new Contract
        {
            Id = 1,
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1))
        };
        
        var service = CreateService();

        _contractRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        Func<Task> action = () => service.AddPaymentAsync(new CreateContractPaymentDto(1, 100), CancellationToken.None);

        await action.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage("Contract expired");
        
        _contractRepository.Verify(x => x.CancelContractAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task AddPayment_WhenAlreadyPaid_ShouldThrow()
    {
        const int totalPaid = 1000;
        
        var service = CreateService();
        
        _contractRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_contract);

        _contractRepository
            .Setup(x => x.GetTotalPaymentsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalPaid);

        Func<Task> action = () => service.AddPaymentAsync(new CreateContractPaymentDto(1, 1), CancellationToken.None);

        await action.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage("Contract already paid");
    }
    
    [Fact]
    public async Task AddPayment_WhenPaymentExceedsRemainingAmount_ShouldThrow()
    {
        const int totalPaid = 900;
        
        var service = CreateService();

        _contractRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_contract);

        _contractRepository
            .Setup(x => x.GetTotalPaymentsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalPaid);

        Func<Task> action = () => service.AddPaymentAsync(new CreateContractPaymentDto(1, 200), CancellationToken.None);

        await action.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage($"Payment exceeds contract's price, {_contract.FinalPrice - totalPaid} left to pay");
    }
}