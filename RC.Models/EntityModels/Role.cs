using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RC.Models.EntityModels
{
    public class Role : IdentityRole<Guid>
    {
        public Role()
        {

        }

        public Role(string name)
        {
            this.Name = name;
        }

        public virtual IList<IdentityUserRole<Guid>> UserRoles { get; set; }

        public virtual IList<IdentityRoleClaim<Guid>> Claims { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }
    }
}
