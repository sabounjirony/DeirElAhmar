using System;
using DM.Models.System.Security;

namespace DM.Models.Setup
{
    public class Address
    {
        public long Id { get; set; }
        public Entity Entity { get; set; }
        public int Sequence { get; set; }
        public int Type { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string Caza { get; set; }
        public string Village { get; set; }
        public string Region { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Phone3 { get; set; }
        public string Fax { get; set; }
        public string PostalCode { get; set; }
        public string PoBox { get; set; }
        public string PlotNum { get; set; }
        public string AreaNum { get; set; }
        public string ExtraData { get; set; }
        public string FullAddress { get; set; }
        public DateTime EntryDate { get; set; }
        //public User User { get; set; }
        public long UserId { get; set; }
        public byte[] Version { get; set; }
    }
}