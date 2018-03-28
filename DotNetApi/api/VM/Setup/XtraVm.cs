using System.Collections.Generic;
using DM.Models.Setup;
using Tools;

namespace VM.Setup
{
    public class XtraVm
    {
        public Xtra Xtra { get; set; }
        public Enumerations.ActionMode ActionMode { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}