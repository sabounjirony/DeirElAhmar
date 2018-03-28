using System;

namespace DM.Models.System
{
    public class Comment
    {
        public long Id { get; set; }
        public string Reference { get; set; }
        public string Text { get; set; }
        //public User User { get; set; }
        public long UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public byte[] Version { get; set; }
    }
}