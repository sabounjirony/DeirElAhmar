using System.Collections.Generic;

namespace DM.Models.System.Security
{
    public class Role
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public Role ParentRole { get; set; }
        public IEnumerable<Role> Children { get; set; }
        public IEnumerable<User> Users { get; set; }
        public bool IsActive { get; set; }
        public string FullParent { get; set; }
        public byte[] Version { get; set; }
    }
}