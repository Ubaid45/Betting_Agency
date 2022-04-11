using BettingAgency.Application.Abstraction.Models;
using BettingAgency.Application.Abstraction.Models.JWT;

namespace BettingAgency.Application.Abstraction.IServices;

public interface IGameService
{
    Task<string> PlaceBet(Request req, CancellationToken ct);
    Task<UserTokens> GetToken(UserLogins userLogins, CancellationToken ct);
    Task<List<UserDto>> GetAllUsers(CancellationToken ct);
}