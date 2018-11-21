using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace RC.Models.Beaer
{
    public interface IJwtIssuerOptions
    {
        string Issuer { get; }
        string Audience { get; }

        TimeSpan TokenValidFor { get; }

        TimeSpan RefreshTokenValidFor { get; }

        DateTime NotBefore { get; }

        DateTime IssuedAt { get; }

        DateTime Expires { get; }

        DateTime RefreshExpires { get; }

        Task<string> JtiGenerator();

        SigningCredentials SigningCredentials { get; }

        SigningCredentials RefreshSigningCredentials { get; }

        //JwtIssuerSettings JwtIssuerSettings { get; }
    }
}
