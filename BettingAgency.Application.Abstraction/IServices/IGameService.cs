using BettingAgency.Application.Abstraction.Models;

namespace BettingAgency.Application.Abstraction.IServices;

public interface IGameService
{
    Task<Response> PlaceBet(Request req, CancellationToken ct);
    Task<List<UserDto>> GetAllUsers(CancellationToken ct);
}