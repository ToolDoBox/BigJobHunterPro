namespace Application.Interfaces;

public interface ILeaderboardNotifier
{
    Task NotifyLeaderboardUpdateAsync(Guid partyId);
}
