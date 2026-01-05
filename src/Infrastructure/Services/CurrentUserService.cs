using System.Security.Claims;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public string? GetUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User == null)
        {
            return null;
        }

        // Extract user ID from NameIdentifier claim
        // (mapped from JWT "sub" claim via NameClaimType configuration in Program.cs)
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userId;
    }

    public async Task<ApplicationUser?> GetCurrentUserAsync()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return null;
        }

        return await _userManager.FindByIdAsync(userId);
    }
}
