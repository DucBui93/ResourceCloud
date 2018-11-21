namespace RC.Models
{
    public class AppSettings
    {
        public BaseUrl BaseUrls { get; set; }
        public JwtIssuerSettings JwtIssuerSettings { get; set; }
    }

    public class JwtIssuerSettings
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string SecretKey { get; set; }

        public string RefreshSecretKey { get; set; }

        public int TokenValidFor { get; set; }

        public int RefreshTokenValidFor { get; set; }

    }
    public class BaseUrl
    {
        public string API { get; set; }
        public string Auth { get; set; }
        public string Web { get; set; }
    }
}
