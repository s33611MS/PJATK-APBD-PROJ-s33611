using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PJATK_APBD_PROJ_s33611.Entities;
using PJATK_APBD_PROJ_s33611.Services;
using PJATK_APBD_PROJ_s33611.Services.Income;

namespace PJATK_APBD_PROJ_s33611.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IncomeController(IIncomeService service) : ControllerBase
{
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent([FromQuery] int? softwareId, [FromQuery] string? currencyCode,CancellationToken cancellationToken)
    {
        return Ok(await service.GetAsync(softwareId, currencyCode, false, cancellationToken));
    }
    
    [HttpGet("expected")]
    public async Task<IActionResult> GetExpected([FromQuery] int? softwareId, [FromQuery] string? currencyCode,CancellationToken cancellationToken)
    {
        return Ok(await service.GetAsync(softwareId, currencyCode, true, cancellationToken));
    }
}