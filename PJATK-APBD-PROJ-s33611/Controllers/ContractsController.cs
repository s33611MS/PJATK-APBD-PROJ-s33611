using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PJATK_APBD_PROJ_s33611.DTOs.Contract;
using PJATK_APBD_PROJ_s33611.Services;

namespace PJATK_APBD_PROJ_s33611.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContractsController(IContractService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await service.GetAllAsync(cancellationToken));
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        return Ok(await service.GetByIdAsync(id, cancellationToken));
    }
    
    [HttpPost]
    public async Task<IActionResult> AddIndividual([FromBody] CreateContractDto request, CancellationToken cancellationToken)
    {
        var client = await service.AddAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
    }
    
    [HttpPost("payments")]
    public async Task<IActionResult> AddCompany([FromBody] CreateContractPaymentDto request, CancellationToken cancellationToken)
    {
        await service.AddPaymentAsync(request, cancellationToken);
        return Created();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}