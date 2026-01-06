using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.HuntingParty;
using Application.Interfaces;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/parties")]
public class HuntingPartiesController : ControllerBase
{
    private readonly IHuntingPartyService _huntingPartyService;

    public HuntingPartiesController(IHuntingPartyService huntingPartyService)
    {
        _huntingPartyService = huntingPartyService;
    }

    /// <summary>
    /// Creates a new hunting party
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(HuntingPartyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateParty([FromBody] CreateHuntingPartyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { error = "Party name is required." });
        }

        if (request.Name.Length > 100)
        {
            return BadRequest(new { error = "Party name cannot exceed 100 characters." });
        }

        try
        {
            var result = await _huntingPartyService.CreatePartyAsync(request);
            return CreatedAtAction(nameof(GetPartyDetail), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Gets the current user's hunting party
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(HuntingPartyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserParty()
    {
        var result = await _huntingPartyService.GetUserPartyAsync();

        if (result == null)
        {
            return NoContent();
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets detailed party information including members
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(HuntingPartyDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPartyDetail(Guid id)
    {
        var result = await _huntingPartyService.GetPartyDetailAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Join a party by invite code
    /// </summary>
    [HttpPost("join")]
    [ProducesResponseType(typeof(HuntingPartyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> JoinParty([FromBody] JoinHuntingPartyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.InviteCode))
        {
            return BadRequest(new { error = "Invite code is required." });
        }

        try
        {
            var result = await _huntingPartyService.JoinPartyAsync(request);

            if (result == null)
            {
                return NotFound(new { error = "Invalid invite code." });
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Leave a party
    /// </summary>
    [HttpDelete("{id:guid}/leave")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LeaveParty(Guid id)
    {
        var result = await _huntingPartyService.LeavePartyAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Gets the party leaderboard ranked by points
    /// </summary>
    [HttpGet("{id:guid}/leaderboard")]
    [ProducesResponseType(typeof(List<LeaderboardEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLeaderboard(Guid id)
    {
        var result = await _huntingPartyService.GetLeaderboardAsync(id);

        if (result.Count == 0)
        {
            // Could be empty party or user not a member
            var party = await _huntingPartyService.GetPartyDetailAsync(id);
            if (party == null)
            {
                return NotFound();
            }
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets rivalry panel data (who's ahead/behind)
    /// </summary>
    [HttpGet("{id:guid}/rivalry")]
    [ProducesResponseType(typeof(RivalryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRivalry(Guid id)
    {
        var result = await _huntingPartyService.GetRivalryAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
