namespace DM.Models.System.MultiLanguage
{
    public class Description
    {
        public long Id { get; set; }
        public string Parent { get; set; }
        public int LanguageId { get; set; }
        public string Code { get; set; }
        public string Text { get; set; }
        public byte[] Version { get; set; }
    }
}