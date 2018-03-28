using System;
using DM.Models.System.Security;

namespace DM.Models.Setup
{
    public class Branch
    {
        public long Id { get; set; }
        public Entity Entity { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Company Company { get; set; }
        public string Status { get; set; }
        public string ExtraData { get; set; }
        //public User User { get; set; }
        public long UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public byte[] Version { get; set; }
    }
}