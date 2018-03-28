namespace DM.Models.System
{
    public class Code
    {
        public string TableName { get; set; }
        public string CodeName { get; set; }
        public string OldCodeName { get; set; }
        public bool Status { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public string Value4 { get; set; }
        public string Value5 { get; set; }
        public string Value6 { get; set; }
        public bool IsProtected { get; set; }
        public string DisplayOrder { get; set; }
        public string RelCode { get; set; }
        public byte[] Version { get; set; }

        protected bool Equals(Code other)
        {
            return TableName == other.TableName && CodeName == other.CodeName;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Code)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = TableName.GetHashCode();
                hashCode = (hashCode * 397) ^ CodeName.GetHashCode();
                return hashCode;
            }
        }
    }
}