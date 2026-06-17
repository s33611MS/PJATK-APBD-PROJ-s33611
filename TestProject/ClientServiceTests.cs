using FluentAssertions;
using Moq;
using PJATK_APBD_PROJ_s33611.DTOs.Client;
using PJATK_APBD_PROJ_s33611.Entities;
using PJATK_APBD_PROJ_s33611.Exceptions;
using PJATK_APBD_PROJ_s33611.Repositories;
using PJATK_APBD_PROJ_s33611.Services;

namespace TestProject;

public class ClientServiceTests
{
    private readonly Mock<IClientRepository> _clientRepository = new();

    [Fact]
    public async Task GetById_ShouldReturnIndividual()
    {
        var client = new IndividualClient
        {
            Id = 1,
            FirstName = "Jan",
            LastName = "Kowalski",
            Pesel = "12345678901",
            Address = "Ulica 1",
            Email = "test@test.pl",
            PhoneNumber = "123456789"
        };

        _clientRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        var service = new ClientService(_clientRepository.Object);
        
        var result = await service.GetByIdAsync(1, CancellationToken.None);
        
        result.Should().BeOfType<IndividualClientResponseDto>();

        var dto = (IndividualClientResponseDto)result;
        
        dto.Pesel.Should().Be("12345678901");
    }
    
    [Fact]
    public async Task GetById_ShouldReturnCompany()
    {
        var client = new CompanyClient
        {
            Id = 1,
            CompanyName = "CompanyName",
            Krs = "1234567890",
            Address = "Ulica 1",
            Email = "test@test.pl",
            PhoneNumber = "123456789"
        };

        _clientRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        var service = new ClientService(_clientRepository.Object);
        
        var result = await service.GetByIdAsync(1, CancellationToken.None);
        
        result.Should().BeOfType<CompanyClientResponseDto>();

        var dto = (CompanyClientResponseDto)result;
        
        dto.Krs.Should().Be("1234567890");
    }
    
    [Fact]
    public async Task GetById_WhenClientDoesNotExist_ShouldThrow()
    {
        _clientRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<int>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        var service = new ClientService(_clientRepository.Object);
        
        Func<Task> action = () => service.GetByIdAsync(1, CancellationToken.None);
        
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task CreateIndividual_ShouldAddClient()
    {
        var dto = new CreateIndividualClientDto(
            "Jan",
            "Kowalski",
            "12345678901",
            "Ulica 1",
            "test@test.pl",
            "123456789"
        );

        Client? createdClient = null;

        _clientRepository
            .Setup(x => x.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()))
            .Callback<Client, CancellationToken>((c, _) => createdClient = c);

        var service = new ClientService(_clientRepository.Object);
        
        await service.AddAsync(dto, CancellationToken.None);
        
        createdClient.Should().NotBeNull();

        createdClient.Should().BeOfType<IndividualClient>();

        var individual = (IndividualClient)createdClient!;
        
        individual.Pesel.Should().Be("12345678901");

        _clientRepository.Verify(x => x.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateIndividual_WithExistingPesel_ShouldThrow()
    {
        var dto = new CreateIndividualClientDto(
            "Jan",
            "Kowalski",
            "12345678901",
            "Ulica 1",
            "test@test.pl",
            "123456789"
        );

        _clientRepository
            .Setup(x => x.PeselExistsAsync(dto.Pesel, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new ClientService(_clientRepository.Object);
        
        Func<Task> action = () => service.AddAsync(dto, CancellationToken.None);
        
        await action.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage($"There already is a client with Pesel: {dto.Pesel}");
    }
    
    [Fact]
    public async Task CreateCompany_ShouldAddClient()
    {
        var dto = new CreateCompanyClientDto(
            "CompanyName",
            "1234567890",
            "Ulica 1",
            "test@test.pl",
            "123456789"
        );

        Client? createdClient = null;

        _clientRepository
            .Setup(x => x.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()))
            .Callback<Client, CancellationToken>((c, _) => createdClient = c);

        var service = new ClientService(_clientRepository.Object);
        
        await service.AddAsync(dto, CancellationToken.None);
        
        createdClient.Should().NotBeNull();

        createdClient.Should().BeOfType<CompanyClient>();

        var company = (CompanyClient)createdClient!;
        
        company.Krs.Should().Be("1234567890");

        _clientRepository.Verify(x => x.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateCompany_WithExistingKrs_ShouldThrow()
    {
        var dto = new CreateCompanyClientDto(
            "CompanyName",
            "1234567890",
            "Ulica 1",
            "test@test.pl",
            "123456789"
        );

        _clientRepository
            .Setup(x => x.KrsExistsAsync(dto.Krs, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new ClientService(_clientRepository.Object);
        
        Func<Task> action = () => service.AddAsync(dto, CancellationToken.None);
        
        await action.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage($"There already is a client with KRS: {dto.Krs}");
    }
    
    [Fact]
    public async Task UpdateClient_ShouldUpdateClient()
    {
        var client = new IndividualClient
        {
            Id = 1,
            FirstName = "Jan",
            LastName = "Kowalski",
            Pesel = "12345678901",
            Address = "Ulica 1",
            Email = "test@test.pl",
            PhoneNumber = "123456789"
        };

        _clientRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);
        
        var dto = new UpdateIndividualClientDto(
            "NewName",
            "Kowalski",
            "Ulica 1",
            "test@test.pl",
            "123456789"
        );

        Client? updatedClient = null;

        _clientRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()))
            .Callback<Client, CancellationToken>((c, _) => updatedClient = c);

        var service = new ClientService(_clientRepository.Object);
        
        await service.UpdateAsync(1, dto, CancellationToken.None);
        
        updatedClient.Should().NotBeNull();

        updatedClient.Should().BeOfType<IndividualClient>();

        var individual = (IndividualClient)updatedClient!;
        
        individual.Pesel.Should().Be("12345678901");
        individual.FirstName.Should().Be("NewName");

        _clientRepository.Verify(x => x.UpdateAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateClient_WithWrongDto_ShouldThrow()
    {
        var client = new IndividualClient
        {
            Id = 1,
            FirstName = "Jan",
            LastName = "Kowalski",
            Pesel = "12345678901",
            Address = "Ulica 1",
            Email = "test@test.pl",
            PhoneNumber = "123456789"
        };

        _clientRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);
        
        var dto = new UpdateCompanyClientDto(
            "NewName",
            "Ulica 1",
            "test@test.pl",
            "123456789"
        );

        var service = new ClientService(_clientRepository.Object);
        
        Func<Task> action = () => service.UpdateAsync(1, dto, CancellationToken.None);
        
        await action.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage($"Client with id: 1 is of different type than requested body");
    }
    
    [Fact]
    public async Task DeleteIndividual_ShouldSetIsDeleted()
    {
        var client = new IndividualClient
        {
            Id = 1,
            IsDeleted = false
        };

        _clientRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        var service = new ClientService(_clientRepository.Object);
        
        await service.DeleteAsync(1, CancellationToken.None);
        
        client.IsDeleted.Should().BeTrue();

        _clientRepository.Verify(x => x.UpdateAsync(client, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteCompany_ShouldThrow()
    {
        var company = new CompanyClient
        {
            Id = 1,
            CompanyName = "CompanyName",
            Krs = "123456789"
        };

        _clientRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);

        var service = new ClientService(_clientRepository.Object);
        
        Func<Task> action = () => service.DeleteAsync(1, CancellationToken.None);
        
        await action.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("You can't delete companies");
    }
}