using BettingAgency.Application.Abstraction.Models;

namespace BettingAgency.Application.Abstraction.IServices;

public interface IGameService
{
    Task<string> PlaceBet(Request req, string email, CancellationToken ct);
    Task<List<UserDto>> GetAllUsers(CancellationToken ct);
}