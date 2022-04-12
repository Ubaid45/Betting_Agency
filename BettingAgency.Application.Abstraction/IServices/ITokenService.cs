using BettingAgency.Application.Abstraction.Models;

namespace BettingAgency.Application.Abstraction.IServices;

public interface ITokenService
{
    Task<string> GetToken(UserLogins userLogins, CancellationToken ct);
}