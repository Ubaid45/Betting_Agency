using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using BettingAgency.Application.Abstraction.IServices;
using BettingAgency.Application.Abstraction.Models;
using BettingAgency.Application.Abstraction.Models.JWT;
using BettingAgency.Application.Exceptions;
using BettingAgency.Persistence;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BettingAgency.Application.Services
{
    public class TokenService
    {
        private readonly IHttpClientFactory _clientFactory;

        private readonly JwtSecurityTokenHandler _tokenHandler;
        
        private readonly JwtSettings _jwtSettings;

        public TokenService(JwtSecurityTokenHandler tokenHandler, IHttpClientFactory clientFactory, IOptions<JwtSettings> settings)
        {
            _tokenHandler = tokenHandler;
            _clientFactory = clientFactory;
            _jwtSettings = settings.Value;
        }

        public async Task<UserModel?> ValidateAuthToken(string token)
        {
            JwtSecurityToken jsonToken = await ValidateTokenSignature(token);

            if (jsonToken == null)
            {
                throw new SecurityTokenException();
            }

            UserModel? payloadModel = JsonSerializer.Deserialize<UserModel>(jsonToken.Payload.SerializeToJson());

            ValidatePayloadAndThrow(payloadModel);

            return payloadModel;
        }

        private void ValidatePayloadAndThrow(UserModel? firebasePayloadModel)
        {
            if (firebasePayloadModel?.Audience != _jwtSettings.Audience)
                throw new HttpStatusException(HttpStatusCode.Unauthorized, nameof(ErrorCodes.InvalidToken));

            if (firebasePayloadModel?.Issuer != _jwtSettings.Issuer)
                throw new HttpStatusException(HttpStatusCode.Unauthorized, nameof(ErrorCodes.InvalidToken));
        }

        private async Task<JwtSecurityToken> ValidateTokenSignature(string token)
        {
            //Download certificates from google
            var client = _clientFactory.CreateClient("JwtTokenClient");

            var jsonResult = await client.GetStringAsync(_jwtSettings.CertificatePath);

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
                    ValidAudience = _jwtSettings.Audience,
                    ValidIssuer = _jwtSettings.Issuer,
                    IssuerSigningKeyResolver = (_, _, _, _) => issuerSigningKeys
                }, out var securityToken);

            if (securityToken.ValidTo < DateTime.UtcNow)
                throw new HttpStatusException(HttpStatusCode.Unauthorized, nameof(ErrorCodes.TokenExpired));

            return securityToken as JwtSecurityToken;
        }
    }
}
