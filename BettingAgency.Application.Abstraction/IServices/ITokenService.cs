using BettingAgency.Application.Abstraction.Models;

namespace BettingAgency.Application.Abstraction.IServices;

public interface ITokenService
{
    Task<UserModel?> ValidateAuthToken(string token);
}