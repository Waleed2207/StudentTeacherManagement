using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace StudentTeacherManagement.Models.Identity
{
    public class Role : IdentityRole
    {
        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
