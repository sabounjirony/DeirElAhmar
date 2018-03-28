using System;
using System.Web;

namespace DM.Models.System
{
    public class Document
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Reference { get; set; }
        public HttpPostedFile File { get; set; }
        //public User User { get; set; }
        public long UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public byte[] Version { get; set; }
    }
}