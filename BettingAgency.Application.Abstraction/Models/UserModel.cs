using System.Text.Json.Serialization;

namespace BettingAgency.Application.Abstraction.Models;

public class UserModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    [JsonPropertyName("aud")]
    public string Audience { get; set; }

    [JsonPropertyName("iss")]
    public string Issuer { get; set; }

    [JsonPropertyName("sub")]
    public string Subject { get; set; }
}