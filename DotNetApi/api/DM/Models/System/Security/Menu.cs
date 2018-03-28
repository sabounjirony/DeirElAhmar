namespace DM.Models.System.Security
{
    public class Menu
    {
        public long Id { get; set; }
        public Menu Parent { get; set; }
        public Module Module { get; set; }
        public string DescriptionCode { get; set; }
        public string Details { get; set; }
        //public Branch Branch { get; set; }
        public long? BranchId { get; set; }
        public string DisplayOrder { get; set; }
        public bool Status { get; set; }
        public byte[] Version { get; set; }
    }
}