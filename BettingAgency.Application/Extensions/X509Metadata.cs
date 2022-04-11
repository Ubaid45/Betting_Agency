using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

namespace BettingAgency.Persistence
{
    public class X509Metadata
    {
        public string KID { get; set; }
        public string Certificate { get; set; }
        public X509SecurityKey X509SecurityKey { get; set; }

        public X509Metadata(string kid, string certificate)
        {
            KID = kid;
            Certificate = certificate;
            X509SecurityKey = BuildSecurityKey(Certificate);
        }

        private X509SecurityKey BuildSecurityKey(string certificate)
        {
            //Remove : -----BEGIN CERTIFICATE----- & -----END CERTIFICATE-----
            var lines = certificate.Split('\n');
            var selectedLines = lines.Skip(1).Take(lines.Length - 3);
            var key = string.Join(Environment.NewLine, selectedLines);

            return new X509SecurityKey(new X509Certificate2(Convert.FromBase64String(key)));
        }
    }
    
    
}