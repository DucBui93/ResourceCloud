using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Models.Enums
{
    public class ResponseStatus
    {
        public enum Status
        {
            Success = 0,
            Fail = 10,
            Invalid = 20,
            Valid = 30,
            Available = 40,
            NotAvailable = 50,
            FailWithException = 60,
            InProgress = 70,
            Completed = 80
        }
    }

    public class JWTInfor
    {
        public const string AudienceId = "AudienceId";
        public const string RefreshTokenKey = "RefreshTokenKey";
    }

    public static class UserStatus
    {
        public const int Inactive = 0;
        public const int Active = 1;
    }
}
