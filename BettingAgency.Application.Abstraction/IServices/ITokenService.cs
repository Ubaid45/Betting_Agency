using BettingAgency.Application.Abstraction.Models;
using BettingAgency.Application.Abstraction.Models.JWT;

namespace BettingAgency.Application.Abstraction.IServices;

public interface ITokenService
{
    Task<string> GetToken(UserLogins userLogins, CancellationToken ct);
}