using System;
using DM.Models.Setup;
using DM.Models.System.Security;

namespace DM.Models.System.Logging
{
    public class LogError
    {
        public long Id { get; set; }
        public int Type { get; set; }
        public int Severity { get; set; }
        public string Source { get; set; }
        public string Text { get; set; }
        //public Branch Branch { get; set; }
        public long BranchId { get; set; }
        //public User User { get; set; }
        public long UserId { get; set; }
        public DateTime EntryDate { get; set; }
    }
}