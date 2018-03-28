using System;

namespace DM.Models.System
{
    public class Help
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string Page { get; set; }
        public string Ctrl { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string DisplayOrder { get; set; }
        //public User User { get; set; }
        public long UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public byte[] Version { get; set; }
    }
}