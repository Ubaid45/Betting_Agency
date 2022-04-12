namespace BettingAgency.Application.Abstraction.Models;
public class JsonWebTokenKeys {
    public string Secret { get; set; }
    public string ValidIssuer { get; set; }
    public string ValidAudience { get; set; }
}