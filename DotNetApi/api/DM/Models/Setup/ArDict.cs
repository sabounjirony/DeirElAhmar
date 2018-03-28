namespace DM.Models.Setup
{
    public class ArDict
    {
        public string Token { get; set; }
        public string Replacement { get; set; }
        public long Occurance { get; set; }
        public long Hit { get; set; }
        public string Trash{ get; set; }
        public byte[] Version { get; set; }
    }
}