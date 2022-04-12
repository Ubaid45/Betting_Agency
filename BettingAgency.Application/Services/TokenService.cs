using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using BettingAgency.Application.Abstraction.IServices;
using BettingAgency.Application.Abstraction.Models;
using BettingAgency.Application.Exceptions;
using BettingAgency.Persistence.Abstraction.Entities;
using BettingAgency.Persistence.Abstraction.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BettingAgency.Application.Services;

public class TokenService : ITokenService
{
    private readonly JsonWebTokenKeys _jsonWebTokenKeys;

    private readonly IMapper _mapper;
    
    private readonly IGameRepository _repository;

    public TokenService(IOptions<JsonWebTokenKeys> settings, IGameRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
        _jsonWebTokenKeys = settings.Value;
    }

    public async Task<string> GetToken(UserLogins userLogins, CancellationToken ct)
    {
        var user = await _repository.GetUserByCredentials(userLogins, ct);
        
        if (user == null) throw new HttpStatusException(HttpStatusCode.NotFound, nameof(ErrorCodes.UserNotFound));
        
        return GenerateToken(_mapper.Map<UserEntity, UserDto>(user));
    }
    
    private string GenerateToken(UserDto user)
    {
        var mySecret = _jsonWebTokenKeys.Secret;
        var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));
        var myIssuer = _jsonWebTokenKeys.ValidIssuer;
        var myAudience = _jsonWebTokenKeys.ValidAudience;
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FullName),
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
            }),
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = myIssuer,
            Audience = myAudience,
            SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}