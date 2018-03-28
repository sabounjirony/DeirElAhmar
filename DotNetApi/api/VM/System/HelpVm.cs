using System.Collections.Generic;
using DM.Models.System;
using Tools;

namespace VM.System
{
    public class HelpVm
    {
        public Help Help { get; set; }
        public Dictionary<string, string> Orders { get; set; }
        public List<Help> Parents { get; set; }
        public string DisplayPage { get; set; }
        public string DisplayControl { get; set; }
        public string Signature { get; set; }
        public Enumerations.ActionMode ActionMode { get; set; }
    }
}