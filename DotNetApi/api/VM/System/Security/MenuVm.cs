using System.Collections.Generic;
using DM.Models.System.Security;
using Tools;

namespace VM.System.Security
{
    public class MenuVm
    {
        public Menu Menu { get; set; }
        public string Signature { get; set; }
        public Dictionary<string, string> Branches { get; set; }
        public Dictionary<string, string> Statuses { get; set; }
        public Enumerations.ActionMode ActionMode { get; set; }
    }
}