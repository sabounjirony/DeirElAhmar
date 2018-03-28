using System.Collections.Generic;
using DM.Models.Setup;
using Tools;

namespace VM.Setup
{
    public class EmployeeVm
    {
        public Employee Employee { get; set; }
        public Enumerations.ActionMode ActionMode { get; set; }
        public string Signature { get; set; }
        public bool HaveMultipleLocations { get; set; }
        public Dictionary<string, string> Branches { get; set; }
        public Dictionary<string, string> Genders { get; set; }
        public Dictionary<string, string> Titles { get; set; }
        public Dictionary<string, string> IdTypes { get; set; }
        public Dictionary<string, string> Maritals { get; set; }
        public Dictionary<string, string> Statuses { get; set; }
        public Dictionary<string, string> Levels { get; set; }
    }
}