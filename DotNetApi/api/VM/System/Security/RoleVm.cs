using System.Collections.Generic;
using DM.Models.System.Security;
using Tools;

namespace VM.System.Security
{
    public class RoleVm
    {
        public Role Role { get; set; }
        public Enumerations.ActionMode ActionMode { get; set; }
        public Dictionary<string, string> ParentRoles { get; set; }
        public string RolePermissions { get; set; }
        public string RoleUsers { get; set; }
    }
}