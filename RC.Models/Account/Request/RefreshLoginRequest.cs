using System.ComponentModel.DataAnnotations;

namespace RC.Models.Account.Request
{
    public class RefreshLoginRequest
    {
        [Required]
        public string RefreshToken { get; set; }

        // un-comment when apply
        // public string FingerPrintHash { get; set; }
    }
}
