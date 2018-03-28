namespace DM.Models.Setup
{
    public class AntiDict
    {
        public string Token { get; set; }
        public string Replacement { get; set; }
        public long Occurance { get; set; }
        public byte[] Version { get; set; }
    }
}