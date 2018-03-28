using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace DM.Conventions
{
    public class RowversionConvention : IVersionConvention
    {
        public void Apply(IVersionInstance instance)
        {
            instance.Column("Version");
            instance.CustomType("BinaryBlob");
            instance.CustomSqlType("timestamp");
            instance.Generated.Always();
            instance.UnsavedValue("null");
        }
    }
}