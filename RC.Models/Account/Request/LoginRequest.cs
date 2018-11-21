using System.ComponentModel.DataAnnotations;

namespace RC.Models.Account.Request
{
    public class LoginRequest
    {
        [Required]
        [MaxLength(256)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(256)]
        public string Password { get; set; }

        /// <summary>
        /// Recaptcha response
        /// </summary>
        //public string Captcha { get; set; }

        public bool Remember { get; set; }

        //public string FingerPrintHash { get; set; }
    }
}
