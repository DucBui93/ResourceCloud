using System.Security.Claims;

namespace RC.Models.Beaer
{
    public class ValidateRefreshTokenResult
    {
        public ResponseMessage ResponseMessage { get; set; }

        public ClaimsPrincipal ClaimsPrincipal { get; set; }

        public string RefreshTokenKey { get; set; }

    }
}
