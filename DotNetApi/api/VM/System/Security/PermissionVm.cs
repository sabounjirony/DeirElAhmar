using System.Collections.Generic;
using DM.Models.System.Security;
using Tools;

namespace VM.System.Security
{
    public class PermissionVm
    {
        public Permission Permission { get; set; }
        public string RolesTree { get; set; }
        public string Signature { get; set; }
        public Dictionary<string, string> Branches { get; set; }
        public Dictionary<string, string> Statuses { get; set; }
        public Enumerations.ActionMode ActionMode { get; set; }
    }
}