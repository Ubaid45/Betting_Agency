using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AutoMapper;
using BettingAgency.Application.Abstraction.IServices;
using BettingAgency.Application.Abstraction.Models;
using BettingAgency.Application.Abstraction.Models.JWT;
using BettingAgency.Application.Exceptions;
using BettingAgency.Persistence;
using BettingAgency.Persistence.Abstraction.Entities;
using BettingAgency.Persistence.Abstraction.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BettingAgency.Application.Services;

public class TokenService : ITokenService
{
    private readonly IHttpClientFactory _clientFactory;

    private readonly JsonWebTokenKeys _jsonWebTokenKeys;

    private readonly JwtSecurityTokenHandler _tokenHandler;
    
    private readonly IMapper _mapper;
    
    private readonly IGameRepository _repository;

    public TokenService(JwtSecurityTokenHandler tokenHandler, IHttpClientFactory clientFactory,
        IOptions<JsonWebTokenKeys> settings, IGameRepository repository, IMapper mapper)
    {
        _tokenHandler = tokenHandler;
        _clientFactory = clientFactory;
        _repository = repository;
        _mapper = mapper;
        _jsonWebTokenKeys = settings.Value;
    }

    public async Task<string> GetToken(UserLogins userLogins, CancellationToken ct)
    {
        var user = await _repository.GetUserByCredentials(userLogins, ct);
        
        if (user == null) return string.Empty;
        
        return GenerateToken(_mapper.Map<UserEntity, UserDto>(user));
    }
    public async Task<UserModel?> ValidateAuthToken(string token)
    {
        var jsonToken = await ValidateTokenSignature(token);

        if (jsonToken == null)
        {
            throw new SecurityTokenException();
        }

        var payloadModel = JsonSerializer.Deserialize<UserModel>(jsonToken.Payload.SerializeToJson());

        ValidatePayloadAndThrow(payloadModel);

        return payloadModel;
    }

    private void ValidatePayloadAndThrow(UserModel? firebasePayloadModel)
    {
        if (firebasePayloadModel?.Audience != _jsonWebTokenKeys.Audience)
        {
            throw new HttpStatusException(HttpStatusCode.Unauthorized, nameof(ErrorCodes.InvalidToken));
        }

        if (firebasePayloadModel?.Issuer != _jsonWebTokenKeys.Issuer)
        {
            throw new HttpStatusException(HttpStatusCode.Unauthorized, nameof(ErrorCodes.InvalidToken));
        }
    }

    private async Task<JwtSecurityToken> ValidateTokenSignature(string token)
    {
        //Download certificates from google
        var client = _clientFactory.CreateClient("JwtTokenClient");

        var jsonResult = await client.GetStringAsync(_jsonWebTokenKeys.CertificatePath);

        //Convert JSON Result
        var x509Metadata = JsonDocument.Parse(jsonResult).RootElement
            .EnumerateObject() // Enumerate its items
            .Select(i => new X509Metadata(i.Name, i.Value.ToString()));

        //Extract IssuerSigningKeys
        var issuerSigningKeys = x509Metadata.Select(s => s.X509SecurityKey);

        _tokenHandler.ValidateToken(token,
            new TokenValidationParameters
            {
                IssuerSigningKeys = issuerSigningKeys,
                ValidAudience = _jsonWebTokenKeys.Audience,
                ValidIssuer = _jsonWebTokenKeys.Issuer,
                IssuerSigningKeyResolver = (_, _, _, _) => issuerSigningKeys
            }, out var securityToken);

        if (securityToken.ValidTo < DateTime.UtcNow)
        {
            throw new HttpStatusException(HttpStatusCode.Unauthorized, nameof(ErrorCodes.TokenExpired));
        }

        return securityToken as JwtSecurityToken;
    }
    
    private string GenerateToken(UserDto user)
    {
        var mySecret = _jsonWebTokenKeys.Secret;
        var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));
        var myIssuer = _jsonWebTokenKeys.Issuer;
        var myAudience = _jsonWebTokenKeys.Audience;
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FullName),
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = myIssuer,
            Audience = myAudience,
            SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}