using PJATK_APBD_PROJ_s33611.DTOs.Client;
using PJATK_APBD_PROJ_s33611.Entities;

namespace PJATK_APBD_PROJ_s33611.Mappers;

public static class ClientMapper
{
    public static ClientResponseDto ToDto(Client client)
    {
        return client switch
        {
            IndividualClient individual => new IndividualClientResponseDto
            (
                individual.Id,
                individual.Address,
                individual.Email,
                individual.PhoneNumber,
                individual.FirstName,
                individual.LastName,
                individual.Pesel
            ),

            CompanyClient company => new CompanyClientResponseDto
            (
                company.Id,
                company.Address,
                company.Email,
                company.PhoneNumber,
                company.CompanyName,
                company.Krs
            ),

            _ => throw new InvalidOperationException()
        };
    }
    
    public static Client ToEntity(CreateClientDto dto)
    {
        return dto switch
        {
            CreateIndividualClientDto individual => new IndividualClient
            {
                Address = individual.Address,
                Email = individual.Email,
                PhoneNumber = individual.PhoneNumber,
                FirstName = individual.FirstName,
                LastName = individual.LastName,
                Pesel = individual.Pesel
            },

            CreateCompanyClientDto company => new CompanyClient
            {
                Address = company.Address,
                Email = company.Email,
                PhoneNumber = company.PhoneNumber,
                CompanyName = company.CompanyName,
                Krs = company.Krs
            },

            _ => throw new InvalidOperationException()
        };
    }
}