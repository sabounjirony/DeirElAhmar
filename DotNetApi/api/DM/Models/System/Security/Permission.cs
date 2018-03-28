using System;
using System.Collections.Generic;

namespace DM.Models.System.Security
{
    public class Permission
    {
        public long Id { get; set; }
        public Module Module { get; set; }
        public string Code { get; set; }
        //public User User { get; set; }
        public long UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public bool IsActiveInherited { get; set; }
        public ICollection<Role> Roles { get; set; }
        public byte[] Version { get; set; }
    }
}