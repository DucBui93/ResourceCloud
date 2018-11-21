using System;
using Microsoft.AspNetCore.Identity;

namespace RC.Models.EntityModels
{
    public class UserToken
    {
        public  Guid Id { get; set; }

        public Guid UserId { get; set; }
        public string RefreshTokenKey { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime ExpirationTime { get; set; }
    }
}
