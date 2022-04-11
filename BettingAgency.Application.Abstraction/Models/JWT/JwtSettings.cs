namespace BettingAgency.Application.Abstraction.Models.JWT;
public class JwtSettings {
    public bool ValidateIssuerSigningKey {
        get;
        set;
    }
    public string IssuerSigningKey {
        get;
        set;
    }
    public bool ValidateIssuer {
        get;
        set;
    } = true;
    public string ValidIssuer {
        get;
        set;
    }
    public bool ValidateAudience {
        get;
        set;
    } = true;
    public string ValidAudience {
        get;
        set;
    }
    public bool RequireExpirationTime {
        get;
        set;
    }
    public bool ValidateLifetime {
        get;
        set;
    } = true;

    public string CertificatePath { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}