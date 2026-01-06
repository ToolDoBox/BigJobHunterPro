using Application.DTOs.HuntingParty;

namespace Application.Interfaces;

public interface IHuntingPartyService
{
    Task<HuntingPartyDto> CreatePartyAsync(CreateHuntingPartyRequest request);
    Task<HuntingPartyDto?> GetUserPartyAsync();
    Task<HuntingPartyDetailDto?> GetPartyDetailAsync(Guid partyId);
    Task<HuntingPartyDto?> JoinPartyAsync(JoinHuntingPartyRequest request);
    Task<bool> LeavePartyAsync(Guid partyId);
    Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(Guid partyId);
    Task<RivalryDto?> GetRivalryAsync(Guid partyId);
    Task<Guid?> GetUserPartyIdAsync(string userId);
}
