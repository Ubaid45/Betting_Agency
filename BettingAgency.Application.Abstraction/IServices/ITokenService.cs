using BettingAgency.Application.Abstraction.Models;
using BettingAgency.Application.Abstraction.Models.JWT;

namespace BettingAgency.Application.Abstraction.IServices;

public interface ITokenService
{
    UserTokens GetToken(UserLogins userLogins, List<UserDto> logins);
    Task<UserModel?> ValidateAuthToken(string token);
}