using System;
using DM.Models.Setup;
using DM.Models.System.Security;

namespace DM.Models.System.Logging
{
    public class Log
    {
        public long Id { get; set; }
        public Module Module { get; set; }
        public string Action { get; set; }
        public string Text { get; set; }
        //public Branch Branch { get; set; }
        public long BranchId { get; set; }
        //public User User { get; set; }
        public long UserId { get; set; }
        public DateTime EntryDate { get; set; }
    }
}