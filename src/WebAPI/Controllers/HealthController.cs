using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public HealthController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            timestampUtc = DateTime.UtcNow
        });
    }

    [HttpGet("db")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDatabaseHealth(CancellationToken cancellationToken)
    {
        var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);

        var response = new
        {
            status = canConnect ? "ok" : "unhealthy",
            database = canConnect ? "connected" : "unreachable",
            timestampUtc = DateTime.UtcNow
        };

        if (!canConnect)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
        }

        return Ok(response);
    }
}
