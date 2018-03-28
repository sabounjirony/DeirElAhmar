using System.Collections.Generic;
using Tools;

namespace VM.System
{
    public class CodeVm
    {
        public Dictionary<string, string> Tables { get; set; }
        public Dictionary<string, string> Statuses { get; set; }
        public Dictionary<string, string> Orders { get; set; }
        public DM.Models.System.Code Code { get; set; }
        public Enumerations.ActionMode ActionMode { get; set; }
    }
}
