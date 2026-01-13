using Application.DTOs.HuntingParty;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public class HuntingPartyService : IHuntingPartyService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMemoryCache _cache;
    private const int CacheExpirationMinutes = 10;

    public HuntingPartyService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        IMemoryCache cache)
    {
        _context = context;
        _currentUserService = currentUserService;
        _cache = cache;
    }

    public async Task<HuntingPartyDto> CreatePartyAsync(CreateHuntingPartyRequest request)
    {
        var userId = _currentUserService.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Check if user already has a party (one party per user for MVP)
        var existingMembership = await _context.HuntingPartyMemberships
            .FirstOrDefaultAsync(m => m.UserId == userId && m.IsActive);

        if (existingMembership != null)
        {
            throw new InvalidOperationException("You can only belong to one hunting party at a time. Leave your current party first.");
        }

        var inviteCode = await GenerateUniqueInviteCodeAsync();

        var party = new HuntingParty
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            InviteCode = inviteCode,
            CreatorId = userId,
            CreatedDate = DateTime.UtcNow
        };

        var membership = new HuntingPartyMembership
        {
            Id = Guid.NewGuid(),
            HuntingPartyId = party.Id,
            UserId = userId,
            Role = PartyRole.Creator,
            JoinedDate = DateTime.UtcNow,
            IsActive = true
        };

        _context.HuntingParties.Add(party);
        _context.HuntingPartyMemberships.Add(membership);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("IX_HuntingParties_InviteCode") == true)
            {
                throw new InvalidOperationException(
                    "Unable to create party due to a conflict. Please try again.");
            }

            if (ex.InnerException?.Message.Contains("IX_HuntingPartyMemberships") == true)
            {
                throw new InvalidOperationException(
                    "You are already a member of a hunting party.");
            }

            throw new InvalidOperationException(
                "Unable to create party. Please try again later.");
        }

        // Invalidate cache for this user
        _cache.Remove($"user-party-id-{userId}");

        return new HuntingPartyDto
        {
            Id = party.Id,
            Name = party.Name,
            InviteCode = party.InviteCode,
            MemberCount = 1,
            CreatedDate = party.CreatedDate,
            IsCreator = true
        };
    }

    public async Task<HuntingPartyDto?> GetUserPartyAsync()
    {
        var userId = _currentUserService.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var membership = await _context.HuntingPartyMemberships
            .Include(m => m.HuntingParty)
            .ThenInclude(p => p.Memberships.Where(m => m.IsActive))
            .FirstOrDefaultAsync(m => m.UserId == userId && m.IsActive);

        if (membership == null)
        {
            return null;
        }

        var party = membership.HuntingParty;
        return new HuntingPartyDto
        {
            Id = party.Id,
            Name = party.Name,
            InviteCode = party.InviteCode,
            MemberCount = party.Memberships.Count(m => m.IsActive),
            CreatedDate = party.CreatedDate,
            IsCreator = party.CreatorId == userId
        };
    }

    public async Task<HuntingPartyDetailDto?> GetPartyDetailAsync(Guid partyId)
    {
        var userId = _currentUserService.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Verify user is a member of this party
        var userMembership = await _context.HuntingPartyMemberships
            .FirstOrDefaultAsync(m => m.HuntingPartyId == partyId && m.UserId == userId && m.IsActive);

        if (userMembership == null)
        {
            return null;
        }

        var party = await _context.HuntingParties
            .Include(p => p.Memberships.Where(m => m.IsActive))
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(p => p.Id == partyId);

        if (party == null)
        {
            return null;
        }

        var members = party.Memberships
            .Where(m => m.IsActive)
            .Select(m => new HuntingPartyMemberDto
            {
                UserId = m.UserId,
                DisplayName = m.User.DisplayName,
                TotalPoints = m.User.TotalPoints,
                JoinedDate = m.JoinedDate,
                Role = m.Role.ToString()
            })
            .OrderByDescending(m => m.TotalPoints)
            .ToList();

        return new HuntingPartyDetailDto
        {
            Id = party.Id,
            Name = party.Name,
            InviteCode = party.InviteCode,
            CreatedDate = party.CreatedDate,
            IsCreator = party.CreatorId == userId,
            Members = members
        };
    }

    public async Task<HuntingPartyDto?> JoinPartyAsync(JoinHuntingPartyRequest request)
    {
        var userId = _currentUserService.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Check if user already has a party
        var existingMembership = await _context.HuntingPartyMemberships
            .FirstOrDefaultAsync(m => m.UserId == userId && m.IsActive);

        if (existingMembership != null)
        {
            throw new InvalidOperationException("You can only belong to one hunting party at a time. Leave your current party first.");
        }

        var party = await _context.HuntingParties
            .Include(p => p.Memberships.Where(m => m.IsActive))
            .FirstOrDefaultAsync(p => p.InviteCode == request.InviteCode.Trim().ToUpperInvariant());

        if (party == null)
        {
            return null; // Invalid invite code
        }

        var membership = new HuntingPartyMembership
        {
            Id = Guid.NewGuid(),
            HuntingPartyId = party.Id,
            UserId = userId,
            Role = PartyRole.Member,
            JoinedDate = DateTime.UtcNow,
            IsActive = true
        };

        _context.HuntingPartyMemberships.Add(membership);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("IX_HuntingPartyMemberships") == true)
            {
                throw new InvalidOperationException(
                    "You are already a member of a hunting party. Leave your current party first.");
            }

            throw new InvalidOperationException(
                "Unable to join party. Please try again later.");
        }

        // Invalidate cache for this user
        _cache.Remove($"user-party-id-{userId}");

        return new HuntingPartyDto
        {
            Id = party.Id,
            Name = party.Name,
            InviteCode = party.InviteCode,
            MemberCount = party.Memberships.Count(m => m.IsActive) + 1,
            CreatedDate = party.CreatedDate,
            IsCreator = false
        };
    }

    public async Task<bool> LeavePartyAsync(Guid partyId)
    {
        var userId = _currentUserService.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var membership = await _context.HuntingPartyMemberships
            .FirstOrDefaultAsync(m => m.HuntingPartyId == partyId && m.UserId == userId && m.IsActive);

        if (membership == null)
        {
            return false;
        }

        // Soft delete - set IsActive to false
        membership.IsActive = false;
        await _context.SaveChangesAsync();

        // Invalidate cache for this user
        _cache.Remove($"user-party-id-{userId}");

        return true;
    }

    public async Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(Guid partyId)
    {
        var userId = _currentUserService.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Verify user is a member of this party
        var userMembership = await _context.HuntingPartyMemberships
            .AnyAsync(m => m.HuntingPartyId == partyId && m.UserId == userId && m.IsActive);

        if (!userMembership)
        {
            return new List<LeaderboardEntryDto>();
        }

        var members = await _context.HuntingPartyMemberships
            .Where(m => m.HuntingPartyId == partyId && m.IsActive)
            .Select(m => new
            {
                m.UserId,
                DisplayName = m.User.DisplayName,
                TotalPoints = m.User.TotalPoints,
                ApplicationCount = _context.Applications.Count(a => a.UserId == m.UserId)
            })
            .OrderByDescending(m => m.TotalPoints)
            .ToListAsync();

        return members.Select((m, index) => new LeaderboardEntryDto
        {
            Rank = index + 1,
            UserId = m.UserId,
            DisplayName = m.DisplayName,
            TotalPoints = m.TotalPoints,
            ApplicationCount = m.ApplicationCount,
            IsCurrentUser = m.UserId == userId
        }).ToList();
    }

    public async Task<RivalryDto?> GetRivalryAsync(Guid partyId)
    {
        var userId = _currentUserService.GetUserId()
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var leaderboard = await GetLeaderboardAsync(partyId);

        if (leaderboard.Count == 0)
        {
            return null;
        }

        var currentUserEntry = leaderboard.FirstOrDefault(e => e.IsCurrentUser);
        if (currentUserEntry == null)
        {
            return null;
        }

        var currentRank = currentUserEntry.Rank;
        var userAhead = currentRank > 1 ? leaderboard[currentRank - 2] : null;
        var userBehind = currentRank < leaderboard.Count ? leaderboard[currentRank] : null;

        return new RivalryDto
        {
            CurrentRank = currentRank,
            TotalMembers = leaderboard.Count,
            UserAhead = userAhead != null ? new RivalInfoDto
            {
                DisplayName = userAhead.DisplayName,
                Points = userAhead.TotalPoints,
                Gap = userAhead.TotalPoints - currentUserEntry.TotalPoints
            } : null,
            UserBehind = userBehind != null ? new RivalInfoDto
            {
                DisplayName = userBehind.DisplayName,
                Points = userBehind.TotalPoints,
                Gap = currentUserEntry.TotalPoints - userBehind.TotalPoints
            } : null
        };
    }

    public async Task<Guid?> GetUserPartyIdAsync(string userId)
    {
        // Generate cache key
        var cacheKey = $"user-party-id-{userId}";

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out Guid? cachedPartyId))
        {
            return cachedPartyId;
        }

        var membership = await _context.HuntingPartyMemberships
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.UserId == userId && m.IsActive);

        var partyId = membership?.HuntingPartyId;

        // Cache for 10 minutes
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
        };

        _cache.Set(cacheKey, partyId, cacheOptions);

        return partyId;
    }

    private async Task<string> GenerateUniqueInviteCodeAsync()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Exclude confusing chars (0, O, 1, I)
        const int maxRetries = 10;

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            var code = new string(Enumerable.Range(0, 8)
                .Select(_ => chars[Random.Shared.Next(chars.Length)])
                .ToArray());

            var exists = await _context.HuntingParties
                .AnyAsync(p => p.InviteCode == code);

            if (!exists)
            {
                return code;
            }
        }

        throw new InvalidOperationException(
            "Unable to generate a unique invite code. Please try again.");
    }
}
