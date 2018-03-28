using System;
using System.Collections.Generic;

namespace DM.Models.Setup
{
    public class Employee
    {
        public long Id { get; set; }
        public Entity Entity { get; set; }
        public string Reference
        {
            get { return "Employee_" + Id; }
        }
        public List<KeyValuePair<long, string>> LocationIds { get; set; }
        public string LiteralLocation { get; set; }
        public string Level { get; set; }
        //public User User { get; set; }
        public long UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public string Status { get; set; }
        public byte[] Version { get; set; }
    }
}