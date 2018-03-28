namespace DM.Models.Setup
{
    public class Xtra
    {
        public virtual string Object { get; set; }
        public long Id { get; set; }
        public string Property { get; set; }
        public string Value { get; set; }
        public byte[] Version { get; set; }

        protected bool Equals(Xtra other)
        {
            return Object == other.Object && Id == other.Id && Property == other.Property;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Xtra)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Object.GetHashCode();
                hashCode = (hashCode * 397) ^ Id.GetHashCode();
                hashCode = (hashCode * 397) ^ Property.GetHashCode();
                return hashCode;
            }
        }
    }
}