using System.Collections.Generic;
using DM.Models.System.Security;
using Tools;

namespace VM.System.Security
{
    public class UserVm
    {
        public User User { get; set; }
        public Dictionary<string, string> Languages { get; set; }
        public Dictionary<string, string> PageSizes { get; set; }
        public Dictionary<string, string> Branches { get; set; }
        public string Signature { get; set; }
        public bool ChangePassword { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string UserRoles { get; set; }
        public Enumerations.ActionMode ActionMode { get; set; }
    }
}