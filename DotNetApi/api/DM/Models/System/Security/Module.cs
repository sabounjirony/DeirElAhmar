using System;
using System.Collections.Generic;

namespace DM.Models.System.Security
{
    public class Module
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public Module SystemModule { get; set; }
        //public User Author { get; set; }
        public long Author { get; set; }
        //public User User { get; set; }
        public long UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public bool EnableLogging { get; set; }
        public string Status { get; set; }
        public IEnumerable<object> Permissions { get; set; }
        public byte[] Version { get; set; }
    }
}