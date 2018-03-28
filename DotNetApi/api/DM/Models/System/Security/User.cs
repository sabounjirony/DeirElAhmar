using System;
using System.Collections.Generic;
using DM.Models.Setup;

namespace DM.Models.System.Security
{
    public class User
    {
        public long Id { get; set; }
        public Entity Entity { get; set; }
        public long Pin { get; set; }
        public string UserName { get; set; }
        public byte[] Password { get; set; }
        public int LanguageId { get; set; }
        public string PasswordHistory { get; set; }
        public DateTime LastPasswordUpdate { get; set; }
        public bool MustChangePassword { get; set; }
        public int PageSize { get; set; }
        public bool IsBlocked { get; set; }
        //public Branch Branch { get; set; }
        public long BranchId { get; set; }
        public long EnteringUserId { get; set; }
        public DateTime EntryDate { get; set; }
        public ICollection<Role> Roles { get; set; }
        public byte[] Version { get; set; }

        public bool IsFullPermission { get; set; }
        public bool IsRestricted { get; set; }
    }
}