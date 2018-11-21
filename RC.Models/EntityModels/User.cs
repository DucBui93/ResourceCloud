using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RC.Models.EntityModels
{
    public class User : IdentityUser<Guid>
    {
        public virtual IList<IdentityUserRole<Guid>> UserRoles { get; set; }

        public virtual IList<IdentityUserClaim<Guid>> UserClaims { get; set; }

        public virtual IList<IdentityUserLogin<Guid>> UserLogins { get; set; }

        public virtual IList<IdentityRoleClaim<Guid>> RoleClaims { get; set; }

        [MaxLength(50)]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [MaxLength(50)]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public int Status { get; set; }
        // audit

        public Guid? CreatorUserId { get; set; }

        public DateTime? CreationTime { get; set; }

        public Guid? LastModifierUserId { get; set; }

        public DateTime? LastModificationTime { get; set; }
    }
}
